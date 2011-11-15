using System;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections.Generic;

using CoreSystem.Util;
using CoreSystem.ValueTypeExtension;

using Feedbook.Model;
using Feedbook.Helper;
using Feedbook.Services.WebFeed;
using System.Windows;
using System.Windows.Threading;
using System.Threading;
using System.Web;


namespace Feedbook.Services.Twitter
{
    internal class TwitterSocialService : SocialService
    {
        private readonly TwitterProcessor feedProcessor;

        public override FeedProcessor Processor
        {
            get { return this.feedProcessor; }
        }

        public override bool IsReplySupported { get { return true; } }

        public override bool IsLikeSupported { get { return false; } }

        public override bool IsShareSupported { get { return true; } }

        public override bool IsFollowSupported { get { return true; } }

        public TwitterSocialService(Account account)
            : base(account)
        {
            if (account.AccountType != AccountType.Twitter)
                throw new ArgumentException(string.Format("Invalid Account Type: '{0}', expecting Twitter Account", account.AccountType));

            this.feedProcessor = new TwitterProcessor(this);
        }

        public override ISocialUser GetUserProfile()
        {
            return new TwitterSocialUser(TwitterService.Show(this.Account.UserName));
        }

        public override ISocialUser[] GetFriends()
        {
            var friends = TwitterService.Friends(this.Token, this.TokenSecret);
            return friends.Select(user => new TwitterSocialUser(user)).ToArray();
        }

        public override IEnumerable<Channel> GetFeeds()
        {
            List<Channel> channels = new List<Channel>();

            yield return new Channel
            {
                Title = "Home / " + this.Account.UserName,
                Description = "Home timeline",
                ChannelId = TwitterService.GetTimelineUrl(Timelines.Home) + "/" + this.Account.UserName, //fake
                DownloadUrl = TwitterService.GetTimelineUrl(Timelines.Home)
            };

            yield return new Channel
            {
                Title = "Replies / " + this.Account.UserName,
                Description = "Replies",
                ChannelId = TwitterService.GetTimelineUrl(Timelines.Replies) + "/" + this.Account.UserName, //fake
                DownloadUrl = TwitterService.GetTimelineUrl(Timelines.Replies)
            };

            yield return new Channel
            {
                Title = "Direct Messages / " + this.Account.UserName,
                Description = "Direct Messages",
                ChannelId = TwitterService.GetTimelineUrl(Timelines.DirectMessages) + "/" + this.Account.UserName, //fake
                DownloadUrl = TwitterService.GetTimelineUrl(Timelines.DirectMessages)
            };

            yield return new Channel
            {
                Title = "Friend's Feed / " + this.Account.UserName,
                Description = "Friend's Feed",
                ChannelId = this.GetFriendsFeedChannelId(),
                DownloadUrl = this.GetFriendsFeedTimelineUrl()
            };
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
                        string maxStatusId = channel.Feeds
                                                 .OrderByDescending(f => f.Updated)
                                                 .Take(1)
                                                 .Select(f => f.Guid)
                                                 .FirstOrDefault();
                        if (maxStatusId != null) maxStatusId = GetStatusId(maxStatusId);

                        if (channel.ChannelId == this.GetFriendsFeedChannelId())
                            channel.DownloadUrl = this.GetFriendsFeedTimelineUrl();

                        string feeds;
                        string downloadUrl = channel.DownloadUrl;

                        if (channel.IsTwitterSearch)
                            feeds = TwitterService.Search(downloadUrl, maxStatusId);
                        else
                            feeds = TwitterService.GetTimeline(downloadUrl, maxStatusId, this.Token, this.TokenSecret);

                        var updatedChannel = this.feedProcessor.Parse(feeds, downloadUrl);
                        var updatedFeeds = new List<Feed>(channel.Feeds);

                        foreach (var feed in updatedChannel.Feeds)
                            if (!updatedFeeds.Any(f => f.Guid == feed.Guid))
                                updatedFeeds.Add(feed);

                        foreach (var feed in updatedFeeds.OrderByDescending(f => f.Updated)
                                                          .Skip(Constants.SysConfig.MaxTwitterFeed)
                                                          .ToArray())
                            updatedFeeds.Remove(feed);

                        dispatcher.BeginInvoke(new Action(() =>
                            {
                                foreach (var feed in channel.Feeds.ToArray())
                                    if (!updatedFeeds.Any(f => f.Guid == feed.Guid))
                                        channel.Feeds.Remove(feed);

                                foreach (var feed in updatedFeeds.OrderByDescending(f => f.Updated))
                                    if (!channel.Feeds.Any(f => f.Guid == feed.Guid))
                                        channel.Feeds.Add(feed);

                                channel.Updated = updatedChannel.Updated;
                            }));

                    }
                    catch (Exception ex)
                    {
                        dispatcher.BeginInvoke(new Action(() =>
                        {
                            if (ex.HttpStatusCode() == HttpStatusCode.Unauthorized)
                                this.LogAndNotify(string.Format("Please refresh your twitter account '{0}' credentials", Account.UserName), ex);
                            else
                                this.WebRequestException(string.Format("WOops! Cannot synchronize twitter feed: '{0}'", channel.Title), ex);
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
            string text = TwitterService.Update(this.Token, this.TokenSecret, message);
            return null;
        }

        public override Feed Update(Feed feed, string message, Attachment[] attachments)
        {
            throw new NotSupportedException("Twitter doesn't support tweet update.");
        }

        public override void Remove(Feed feed)
        {
            Guard.CheckNull(feed, "Remove(feed)");
            TwitterService.Distory(GetStatusId(feed.Guid), this.Token, this.TokenSecret);
        }

        public override void Share(Feed feed)
        {
            Guard.CheckNull(feed, "Share(feed)");
            TwitterService.Retweet(GetStatusId(feed.Guid), this.Token, this.TokenSecret);
        }

        public override Feed Reply(Feed feed, string comments)
        {
            if (comments != null && feed.People.Count > 0 && !comments.Contains(feed.People[0].UserId))
                comments = string.Format("@{0} {1}", feed.People[0].UserId, comments);

            string value = TwitterService.Update(this.Token, this.TokenSecret, comments);
            return null;
        }

        public override void Like(Feed feed)
        {
            throw new NotSupportedException("Like on tweet is not supported");
        }

        public override void Follow(Person person)
        {
            Guard.CheckNull(person, "Follow(person)");
            TwitterService.Follow(person.UserId, Token, TokenSecret);
        }

        public override void UnFollow(Person person)
        {
            Guard.CheckNull(person, "UnFollow(person)");
            TwitterService.UnFollow(person.UserId, Token, TokenSecret);
        }

        public static string GetStatusId(string feedId)
        {
            Guard.CheckNullOrEmpty(feedId, "GetStatusId(feedId)");
            int index;
            long statusId;

            if ((index = feedId.LastIndexOf('/')) != -1
                && index + 1 < feedId.Length
                && long.TryParse(feedId.Substring(index + 1), out statusId))
                return statusId.ToString();

            if ((index = feedId.LastIndexOf(':')) != -1
               && index + 1 < feedId.Length
               && long.TryParse(feedId.Substring(index + 1), out statusId))
                return statusId.ToString();

            throw new InvalidOperationException(string.Format("Unable to parse twitter status id from feed id '{0}'", feedId));
        }

        private string GetFriendsFeedChannelId()
        {
            return TwitterService.GetTimelineUrl(Timelines.Friends) + "/" + "Feed/" + this.Account.UserName;
        }

        private string GetFriendsFeedTimelineUrl()
        {
            var friendList = string.Join("+OR+", this.GetFriends().Select(u => HttpUtility.UrlEncode("to:" + u.UserName)));
            return "http://search.twitter.com/search.atom?q=" + friendList;
        }
    }
}
