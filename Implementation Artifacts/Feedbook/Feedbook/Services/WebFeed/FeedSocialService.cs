using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreSystem.Util;
using Feedbook.Model;
using Feedbook.Helper;
using System.Windows.Threading;
using System.Threading;
using CoreSystem.ValueTypeExtension;
using System.Windows;
using System.Net;

namespace Feedbook.Services.WebFeed
{
    internal class FeedSocialService : SocialService
    {
        private FeedProcessor processor = new FeedProcessor();

        public override FeedProcessor Processor { get { return this.processor; } }

        public override bool IsReplySupported { get { return false; } }

        public override bool IsLikeSupported { get { return false; } }

        public override bool IsShareSupported { get { return false; } }

        public override bool IsFollowSupported { get { return false; } }

        public override ISocialUser GetUserProfile()
        {
            throw new NotImplementedException();
        }

        public override ISocialUser[] GetFriends()
        {
            throw new NotSupportedException();
        }

        public override IEnumerable<Channel> GetFeeds()
        {
            throw new NotSupportedException();
        }

        public override void AsyncUpdate(Channel channel)
        {
            Guard.CheckNull(channel, "GetFeedUpdate(channel)");

            lock (channel)
            {
                if (channel.IsSynchronizing) return;
                channel.IsSynchronizing = true;                    
            }

            Dispatcher dispatcher = Util.GetDispatcher();
            ThreadPool.QueueUserWorkItem(new WaitCallback(
                (object o) =>
                {
                    try
                    {
                        string xmlString = Downloader.DownloadString(channel.DownloadUrl);
                        var updatedChannel = this.processor.Parse(xmlString, channel.DownloadUrl);

                        dispatcher.BeginInvoke(new Action(() =>
                        {
                            List<Feed> feeds = new List<Feed>(channel.Feeds);
                            foreach (var feed in updatedChannel.Feeds)
                            {
                                var existingFeed = feeds.FirstOrDefault(f => f.Guid == feed.Guid);
                                if (existingFeed != null)
                                {
                                    if (existingFeed.Updated.GetUnixTime() < feed.Updated.GetUnixTime())
                                    {
                                        feeds.Add(feed);
                                        feeds.Remove(existingFeed);
                                        //channel.NewFeedCount += 1;
                                    }
                                }
                                else
                                {
                                    feeds.Add(feed);
                                    //channel.NewFeedCount += 1;
                                }
                            }

                            foreach (var feed in feeds.OrderByDescending(f => f.Updated)
                                                              .Skip(Constants.SysConfig.MaxWebFeed)
                                                              .ToArray())
                                feeds.Remove(feed);


                            foreach (var feed in channel.Feeds.ToArray())
                                if (!feeds.Contains(feed))
                                    channel.Feeds.Remove(feed);

                            foreach (var feed in feeds.OrderByDescending(f => f.Updated))
                                if (!channel.Feeds.Contains(feed))
                                {
                                    channel.Feeds.Add(feed);
                                    channel.NewFeedCount += 1;
                                }

                            channel.Updated = updatedChannel.Updated;
                        }));

                    }
                    catch (Exception ex)
                    {
                        dispatcher.BeginInvoke(new Action(() => this.WebRequestException(string.Format("WOops! Cannot synchronize channel: {0}", channel.Title), ex)));
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
            throw new NotSupportedException();
        }

        public override Feed Update(Feed feed, string message, Attachment[] attachments)
        {
            throw new NotSupportedException();
        }

        public override Feed Reply(Feed feed, string comments)
        {
            throw new NotSupportedException();
        }

        public override void Like(Feed feed)
        {
            throw new NotSupportedException();
        }

        public override void Remove(Feed feed)
        {
            throw new NotSupportedException();
        }

        public override void Share(Feed feed)
        {
            throw new NotSupportedException();
        }

        public override void Follow(Person person)
        {
            throw new NotSupportedException();
        }

        public override void UnFollow(Person person)
        {
            throw new NotSupportedException();
        }
    }
}
