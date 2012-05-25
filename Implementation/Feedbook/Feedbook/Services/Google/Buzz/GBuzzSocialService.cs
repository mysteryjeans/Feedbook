using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Feedbook.Model;
using CoreSystem.Util;
using Feedbook.Helper;
using CoreSystem.RefTypeExtension;
using System.Xml.Linq;
using CoreSystem.ValueTypeExtension;
using Feedbook.Services.WebFeed;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Net;

namespace Feedbook.Services.Google.Buzz
{
    internal class GBuzzSocialService : SocialService
    {
        private Dispatcher dispatcher;

        private GBuzzProcessor gbuzzProcessor;

        public override FeedProcessor Processor
        {
            get { return this.gbuzzProcessor; }
        }

        public override bool IsReplySupported { get { return true; } }

        public override bool IsLikeSupported { get { return true; } }

        public override bool IsShareSupported { get { return false; } }

        public override bool IsFollowSupported { get { return true; } }

        public GBuzzSocialService(Account account)
            : base(account)
        {
            if (account.AccountType != AccountType.Google)
                throw new ArgumentException(string.Format("Invalid type of account: '{0}', expecting google(buzz) account", account.AccountType));

            this.gbuzzProcessor = new GBuzzProcessor(this);
        }

        public GBuzzSocialService(Account account, Dispatcher dispatcher)
            : this(account)
        {
            this.dispatcher = dispatcher;
        }

        public override ISocialUser GetUserProfile()
        {
            return new GBuzzUser(GBuzzService.GetUserProfile(this.Token, this.TokenSecret));
        }

        public override ISocialUser[] GetFriends()
        {
            var contacts = GBuzzService.GetFriends(this.Token, this.TokenSecret);
            if (contacts != null)
                return contacts.Select(contact => new GBuzzUser(contact)).ToArray();

            return null;
        }

        public override IEnumerable<Channel> GetFeeds()
        {
            var activityTypes = new ActivityType[] { ActivityType.Consumption, ActivityType.Public, ActivityType.Self };  //Enum.GetValues(typeof(ActivityType));
            foreach (ActivityType activityType in activityTypes)
            {
                Channel activityFeed = new Channel
                {
                    DownloadUrl = GBuzzService.GetActivityFeedUrl(activityType),
                    Title = activityType + " / " + this.Account.FullName,
                    ChannelId = GBuzzService.GetActivityFeedUrl(activityType).Replace("@me", this.Account.UserName) //making unique channel id
                };
                yield return activityFeed;
            }
        }

        public override void AsyncUpdate(Channel channel)
        {
            Guard.CheckNull(channel, "AsyncUpdate(channel)");

            lock (channel)
            {
                if (channel.IsSynchronizing)
                    return;
                channel.IsSynchronizing = true;
            }

            Dispatcher dispatcher = Util.GetDispatcher();
            ThreadPool.QueueUserWorkItem(new WaitCallback(
                (object o) =>
                {
                    try
                    {
                        var updatedChannel = this.gbuzzProcessor.Parse(GBuzzService.GetActivityFeed(channel.DownloadUrl, this.Token, this.TokenSecret), channel.DownloadUrl);

                        dispatcher.BeginInvoke(new Action(() =>
                        {
                            foreach (var feed in updatedChannel.Feeds.OrderByDescending(f => f.Updated))
                            {
                                var existingFeed = channel.Feeds.FirstOrDefault(f => f.Guid == feed.Guid);
                                if (existingFeed != null)
                                {
                                    if (existingFeed.Updated.GetUnixTime() < feed.Updated.GetUnixTime())
                                    {
                                        channel.Feeds.Add(feed);
                                        channel.Feeds.Remove(existingFeed);
                                    }
                                    else
                                    {
                                        existingFeed.Likes = feed.Likes;
                                        existingFeed.IsLiked = feed.IsLiked;

                                        //removing comments if deleted from original post
                                        foreach (var comment in existingFeed.Comments.ToArray())
                                            if (!feed.Comments.Any(c => c.Guid == comment.Guid))
                                                existingFeed.Comments.Remove(comment);

                                        //updating or adding new comments
                                        foreach (var comment in feed.Comments)
                                        {
                                            var existingComment = existingFeed.Comments.FirstOrDefault(c => c.Guid == comment.Guid);
                                            if (existingComment != null)
                                            {
                                                if (existingComment.Published.GetUnixTime() < comment.Published.GetUnixTime()
                                                    //Buzz didn't update publish date if someone edit his comments
                                                    || existingComment.Content != comment.Content)
                                                {
                                                    existingFeed.Comments.Add(comment);
                                                    existingFeed.Comments.Remove(existingComment);
                                                }
                                            }
                                            else
                                                existingFeed.Comments.Add(comment);
                                        }
                                    }
                                }
                                else
                                    channel.Feeds.Add(feed);
                            }

                            foreach (var feed in channel.Feeds.OrderByDescending(f => f.Updated)
                                                              .Skip(Constants.SysConfig.MaxBuzzFeed)
                                                              .ToArray())
                                channel.Feeds.Remove(feed);

                            channel.Updated = updatedChannel.Updated;
                        }));

                    }
                    catch (Exception ex)
                    {
                        dispatcher.BeginInvoke(new Action(() =>
                        {
                            if (ex.HttpStatusCode() == HttpStatusCode.Unauthorized)
                                this.LogAndNotify(string.Format("Please refresh your google buzz account '{0}' credentials", Account.FullName), ex);
                            else
                                this.WebRequestException(string.Format("WOops! Cannot synchronize google buzz feed: '{0}'", channel.Title), ex);
                        }));
                    }
                    finally
                    {
                        dispatcher.BeginInvoke(new Action(() =>
                        {
                            lock (channel)
                            {
                                channel.IsSynchronizing = false;
                            }
                        }));
                    }
                }));
        }

        public override Feed Post(string message, Attachment[] attachments)
        {
            String atomEntry;
            XElement atomElement;

            atomEntry = GetAtomEntryForNote(message, attachments);
            atomEntry = GBuzzService.CreateActivity(atomEntry, this.Token, this.TokenSecret);
            atomElement = XElement.Parse(atomEntry);

            return this.gbuzzProcessor.ParseEntry(atomElement, null);
        }

        public override Feed Update(Feed feed, string message, Attachment[] attachments)
        {
            throw new NotImplementedException();
        }

        public override void Remove(Feed feed)
        {
            Guard.CheckNull(feed, "Remove(feed)");
            GBuzzService.DeleteAcivity(feed.Guid, this.Token, this.TokenSecret);
        }

        public override void Share(Feed feed)
        {
            throw new NotSupportedException("Google Buzz service didn't expose activity sharing API yet ...");
        }

        public override Feed Reply(Feed feed, string comments)
        {
            Guard.CheckNull(feed, "Reply(feed)");
            Guard.CheckNullOrEmpty(comments, "Reply(comments)");

            Person activityUser = feed.Author;

            if (activityUser == null)
                throw new InvalidOperationException("Unable to find user of this activity");

            string atomEntry = GetAtomEntryForComments(comments);
            atomEntry = GBuzzService.CreateComments(atomEntry, activityUser.UserId, feed.Guid, this.Token, this.TokenSecret);

            return this.gbuzzProcessor.ParseEntry(XElement.Parse(atomEntry), null);
        }

        public override void Like(Feed feed)
        {
            Guard.CheckNull(feed, "Reply(feed)");

            var dispatcher = this.dispatcher ?? Util.GetDispatcher();
            if (feed.IsLiked)
            {
                GBuzzService.UnLikeAcivity(feed.Guid, this.Token, this.TokenSecret);
                dispatcher.BeginInvoke(new Action(() =>
                {
                    feed.Likes -= 1;
                    feed.IsLiked = false;
                }));
            }
            else
            {
                var entry = XElement.Parse(GBuzzService.LikeAcivity(feed.Guid, this.Token, this.TokenSecret));
                var updatedFeed = this.gbuzzProcessor.ParseEntry(entry, null);
                dispatcher.BeginInvoke(new Action(() =>
                {
                    feed.Likes = updatedFeed.Likes;
                    feed.IsLiked = updatedFeed.IsLiked;
                }));
            }
        }

        public override void Follow(Person person)
        {
            Guard.CheckNull(person, "Follow(person)");
            GBuzzService.Follow(person.UserId, Token, TokenSecret);
        }

        public override void UnFollow(Person person)
        {
            Guard.CheckNull(person, "UnFollow(person)");
            GBuzzService.UnFollow(person.UserId, Token, TokenSecret);
        }

        public static string GetAtomEntryForComments(string comments)
        {
            Guard.CheckNullOrEmpty(comments, "GetAtomEntryForComments(message)");

            List<object> elements = new List<object>();

            XElement atomEntry = new XElement(FeedProcessor.atomNS + FeedProcessor.XML_ENTRY,
                                              new XElement(FeedProcessor.atomNS + FeedProcessor.XML_CONTENT, new XAttribute(FeedProcessor.XML_TYPE, "text/html"), comments));

            return atomEntry.ToString();
        }

        public static string GetAtomEntryForNote(string message, Attachment[] attachments)
        {
            Guard.CheckNullOrEmpty(message, "GetAtomEntryForNote(message)");

            List<object> elements = new List<object>();
            var activityNote = new XElement(FeedProcessor.activityNS + FeedProcessor.XML_OBJECT_TYPE, FeedProcessor.NAMESPACE_ACTIVITY_STREAMS_NOTE);
            var atomContent = new XElement(FeedProcessor.atomNS + FeedProcessor.XML_CONTENT, new XAttribute(FeedProcessor.XML_TYPE, "text/html"), message);

            elements.Add(activityNote);
            elements.Add(atomContent);
            elements.AddRange(GetBuzzAttachments(attachments));

            XElement atomEntry = new XElement(FeedProcessor.atomNS + FeedProcessor.XML_ENTRY,
                                              new XElement(FeedProcessor.atomNS + FeedProcessor.XML_CONTENT, new XAttribute(FeedProcessor.XML_TYPE, "text/html"), message),
                                              new XElement(FeedProcessor.activityNS + FeedProcessor.XML_OBJECT, elements.ToArray()));

            return atomEntry.ToString();
        }

        private static IEnumerable<object> GetBuzzAttachments(Attachment[] attachments)
        {
            if (attachments != null)
            {
                foreach (Attachment attachment in attachments)
                {
                    switch (attachment.SocialType)
                    {
                        case SocialType.Photo:
                            {
                                XElement buzzAttachment = new XElement(GBuzzProcessor.buzzNS + GBuzzProcessor.XML_ATTACHMENT);
                                buzzAttachment.Add(new XElement(FeedProcessor.activityNS + FeedProcessor.XML_OBJECT_TYPE, FeedProcessor.NAMESPACE_ACTIVITY_STREAMS_PHOTO));

                                if (!string.IsNullOrEmpty(attachment.Title))
                                    buzzAttachment.Add(new XElement(FeedProcessor.atomNS + FeedProcessor.XML_TITLE, attachment.Title));

                                attachment.Links.ForEach(l =>
                                                {
                                                    buzzAttachment.Add(new XElement(FeedProcessor.atomNS + FeedProcessor.XML_LINK,
                                                    new XAttribute(FeedProcessor.XML_REL, l.Rel),
                                                    new XAttribute(FeedProcessor.XML_HREF, l.HRef),
                                                    new XAttribute(FeedProcessor.XML_TYPE, l.Type)));
                                                });

                                yield return buzzAttachment;
                            }
                            break;
                        case SocialType.Video:
                            {
                                XElement buzzAttachment = new XElement(GBuzzProcessor.buzzNS + GBuzzProcessor.XML_ATTACHMENT);
                                buzzAttachment.Add(new XElement(FeedProcessor.activityNS + FeedProcessor.XML_OBJECT_TYPE, FeedProcessor.NAMESPACE_ACTIVITY_STREAMS_VIDEO));
                                if (!string.IsNullOrEmpty(attachment.Title))
                                    buzzAttachment.Add(new XElement(FeedProcessor.atomNS + FeedProcessor.XML_TITLE, attachment.Title));

                                attachment.Links.ForEach(l =>
                                {
                                    buzzAttachment.Add(new XElement(FeedProcessor.atomNS + FeedProcessor.XML_LINK,
                                    new XAttribute(FeedProcessor.XML_REL, l.Rel),
                                    new XAttribute(FeedProcessor.XML_HREF, l.HRef),
                                    new XAttribute(FeedProcessor.XML_TYPE, l.Type)));
                                });

                                yield return buzzAttachment;
                            }
                            break;
                        case SocialType.Article:
                            {
                                XElement buzzAttachment = new XElement(GBuzzProcessor.buzzNS + GBuzzProcessor.XML_ATTACHMENT);
                                buzzAttachment.Add(new XElement(FeedProcessor.activityNS + FeedProcessor.XML_OBJECT_TYPE, FeedProcessor.NAMESPACE_ACTIVITY_STREAMS_ARTICLE));
                                if (!string.IsNullOrEmpty(attachment.Title))
                                    buzzAttachment.Add(new XElement(FeedProcessor.atomNS + FeedProcessor.XML_TITLE, attachment.Title));

                                attachment.Links.ForEach(l =>
                                {
                                    buzzAttachment.Add(new XElement(FeedProcessor.atomNS + FeedProcessor.XML_LINK,
                                    new XAttribute(FeedProcessor.XML_REL, l.Rel),
                                    new XAttribute(FeedProcessor.XML_HREF, l.HRef),
                                    new XAttribute(FeedProcessor.XML_TYPE, l.Type)));
                                });

                                yield return buzzAttachment;
                            }
                            break;
                        default:
                            throw new NotSupportedException(string.Format("Google buzz service does not support attachment of type: '{0}'", attachment.SocialType));
                    }
                }
            }
        }
    }
}
