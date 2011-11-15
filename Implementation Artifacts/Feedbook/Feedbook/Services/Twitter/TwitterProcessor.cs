using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Feedbook.Helper;
using Feedbook.Model;
using CoreSystem.Util;
using System.Xml.Linq;
using CoreSystem.RefTypeExtension;
using Feedbook.Services.WebFeed;

namespace Feedbook.Services.Twitter
{
    internal class TwitterProcessor : FeedProcessor
    {
        private static readonly Dictionary<string, Person> personCache = new Dictionary<string, Person>();

        private readonly TwitterSocialService service;

        public TwitterProcessor(TwitterSocialService service)
        {
            Guard.CheckNull(service, "TwitterFeedProcessor(service)");

            this.service = service;
        }

        public override Channel ParseAtom(XElement feedElement, string downloadUrl)
        {
            var channel = base.ParseAtom(feedElement, downloadUrl);

            if (channel.DownloadUrl == TwitterService.GetTimelineUrl(Timelines.DirectMessages) && channel.Feeds.Count > 0)
                this.UpdateFeedAuthors(channel);

            return channel;
        }

        private void UpdateFeedAuthors(Channel channel)
        {
            try
            {
                string minStatusId = channel.Feeds
                                                 .OrderBy(f => f.Updated)
                                                 .Take(1)
                                                 .Select(f => f.Guid)
                                                 .FirstOrDefault();
                if (minStatusId != null)
                {
                    minStatusId = TwitterSocialService.GetStatusId(minStatusId);
                    minStatusId = (long.Parse(minStatusId) - 1).ToString();
                }

                string dirmessages = TwitterService.GetTimeline("http://api.twitter.com/1/direct_messages.xml", minStatusId, this.service.Token, this.service.TokenSecret);
                XElement element = XElement.Parse(dirmessages);
                foreach (var msgElement in element.Elements("direct_message"))
                {
                    var feed = channel.Feeds.FirstOrDefault(f => TwitterSocialService.GetStatusId(f.Guid) == (string)msgElement.Element("id"));
                    if (feed != null && feed.Author != null)
                    {
                        var author = feed.Author;
                        author.UserId = (string)msgElement.Element("sender_screen_name");
                        author.ImageUrl = (string)msgElement.Element("sender").Element("profile_image_url");
                    }
                }
            }
            catch (Exception ex)
            {
                this.Log("Error updating authors", ex);
            }
        }

        public override Feed ParseEntry(XElement entry, IEnumerable<Person> people)
        {
            Feed feed = base.ParseEntry(entry, people);

            string accountScreenName = this.service.Account.UserName;
            foreach (var person in feed.People)
            {
                string screenName = GetScreenName(feed);

                if (screenName != null)
                {
                    person.UserId = screenName;
                    person.ImageUrl = "http://api.twitter.com/1/users/profile_image/" + screenName;

                    //Person friend = null;

                    //lock (personCache)
                    //{
                    //    if (personCache.ContainsKey(screenName))
                    //        friend = personCache[screenName];
                    //}

                    //if (friend == null)
                    //{
                    //    var user = TwitterService.Show(screenName);
                    //    friend = new Person
                    //    {
                    //        UserId = user.ScreenName,
                    //        IsProtected = user.IsProtected,
                    //        ImageUrl = user.ProfileImageLocation
                    //    };

                    //    lock (personCache)
                    //    {
                    //        if (!personCache.ContainsKey(screenName))
                    //            personCache.Add(screenName, friend);
                    //    }
                    //}

                    //feed.IsProtected = friend.IsProtected;
                    //person.UserId = friend.UserId;
                    //person.ImageUrl = friend.ImageUrl;
                }
            }

            return feed;
        }

        private static string GetScreenName(Feed feed)
        {
            int index;
            if (feed != null)
            {
                if (feed.Guid != null && (index = feed.Guid.IndexOf("http://twitter.com/")) != -1)
                {
                    var screenNameIndex = feed.Guid.IndexOf('/', index + "http://twitter.com/".Length + 1);
                    if (screenNameIndex != -1)
                    {
                        var startIndex = index + "http://twitter.com/".Length;
                        return feed.Guid.Substring(startIndex, screenNameIndex - startIndex);
                    }
                }

                if (feed.People.Count > 0)
                {
                    var author = feed.People.First();
                    if ((index = author.Url.IndexOf("http://twitter.com/")) != -1)
                    {
                        var startIndex = index + "http://twitter.com/".Length;
                        return author.Url.Substring(startIndex);
                    }
                }
            }

            return null;
        }

        public override Icon GetDefIcon(string downloadUrl)
        {
            return new Icon
              {
                  DownloadUrl = Constants.RESX_TWITTER_IMAGE_URI,
                  Title = "Twitter Image",
                  Description = "Feed Icon"
              };
        }
    }
}
