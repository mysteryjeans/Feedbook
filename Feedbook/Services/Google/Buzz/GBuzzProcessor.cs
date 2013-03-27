using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Feedbook.Helper;
using Feedbook.Model;
using System.Xml.Linq;
using CoreSystem.RefTypeExtension;
using CoreSystem.Util;
using Feedbook.Services.WebFeed;

namespace Feedbook.Services.Google.Buzz
{
    internal class GBuzzProcessor : FeedProcessor
    {
        #region Constants

        public const string NAMESPACE_THREAD = "http://purl.org/syndication/thread/1.0";
        public const string NAMESPACE_GOOGLE_BUZZ = "http://schemas.google.com/buzz/2010";
        public const string NAMESPACE_GOOGLE_BUZZ_LIKED = "http://schemas.google.com/buzz/2010#liked";

        #region Thread Tags

        public const string XML_COUNT = "count";

        #endregion

        #region Google Buzz Tags

        public const string XML_ATTACHMENT = "attachment";

        #endregion

        #endregion

        protected static readonly XNamespace thrNS = XNamespace.Get(NAMESPACE_THREAD);
        public static readonly XNamespace buzzNS = XNamespace.Get(NAMESPACE_GOOGLE_BUZZ);

        private readonly GBuzzSocialService service;

        public GBuzzProcessor(GBuzzSocialService service)
        {
            Guard.CheckNull(service, "GBuzzProcessor(service)");

            this.service = service;
        }

        public override Feed ParseEntry(XElement entry, IEnumerable<Person> people)
        {
            if ((string)entry.Element(activityNS + XML_OBJECT_TYPE) == NAMESPACE_ACTIVITY_STREAMS_COMMENT
                 && entry.Element(atomNS + XML_TITLE) == null)
                entry.Add(new XElement(atomNS + XML_TITLE, (string)entry.Element(atomNS + XML_CONTENT)));

            var feed = base.ParseEntry(entry, people);

            var htmlLink = (from link in entry.Elements(atomNS + XML_LINK)
                            where (string)link.Attribute(XML_TYPE) == "text/html" && (string)link.Attribute(XML_REL) == "alternate"
                            select link).SingleOrDefault();

            if (htmlLink != null)
                feed.Link = GetLink(htmlLink);

            //Buzz Like Attribute(buzz:count) is define in link of type poco+xml and rel http://schemas.google.com/buzz/2010#liked
            var buzzLikedLink = entry.Elements(atomNS + XML_LINK).Where(l => (string)l.Attribute(XML_REL) == NAMESPACE_GOOGLE_BUZZ_LIKED && (string)l.Attribute(XML_TYPE) == "application/poco+xml")
                                      .SingleOrDefault();

            if (buzzLikedLink != null && buzzLikedLink.Attribute(buzzNS + XML_COUNT) != null)
            {
                if ((feed.Likes = (int)buzzLikedLink.Attribute(buzzNS + XML_COUNT)) > 0)
                {
                    var peopleLiked = GBuzzService.ParsePortableContacts(GBuzzService.OAuthGetWebRequest((string)buzzLikedLink.Attribute(XML_HREF), this.service.Token, this.service.TokenSecret));

                    feed.IsLiked = (peopleLiked != null && peopleLiked.Count(c => c.Id == this.service.UserId) > 0);
                }
            }

            var commentLink = entry.Elements(atomNS + XML_LINK).Where(l => (string)l.Attribute(XML_REL) == "replies" && (string)l.Attribute(XML_TYPE) == "application/atom+xml")
                                                               .SingleOrDefault();
            if (commentLink != null)
            {
                var repliesCount = commentLink.Attribute(thrNS + XML_COUNT);
                if (repliesCount != null && (int)repliesCount > 0)
                {
                    string commentsUrl = (string)commentLink.Attribute(XML_HREF);
                    string commentFeeds = GBuzzService.GetActivityFeed(commentsUrl, this.service.Token, this.service.TokenSecret);

                    XElement comments = XElement.Parse(commentFeeds);
                    comments.Add(new XElement(atomNS + XML_UPDATED, DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")));
                    comments.Elements(atomNS + XML_ENTRY)
                            .ForEach(e =>
                            {
                                e.Add(new XElement(atomNS + XML_TITLE, Content.ToPainText((string)e.Element(atomNS + XML_CONTENT))));
                                e.Add(new XElement(atomNS + XML_UPDATED, (string)e.Element(atomNS + XML_PUBLISHED)));
                            });

                    var channel = this.Parse(comments.ToString(), commentsUrl);
                    channel.Feeds.ForEach(f =>
                        {
                            var person = f.People.First();
                            var comment = new Comment
                            {
                                Guid = f.Guid,
                                Content = f.EncodedDescription,
                                Person = new Person { Name = person.Name, UserId = person.UserId, ImageUrl = person.ImageUrl, Url = person.Url, Email = person.Email, Role = person.Role },
                                Published = f.Updated
                            };

                            feed.Comments.Add(comment);
                        });
                }
            }

            // Finding buzz attachments 
            var activityObject = (from activityObj in entry.Elements(activityNS + XML_OBJECT)
                                  where (string)activityObj.Element(activityNS + XML_OBJECT_TYPE) == NAMESPACE_ACTIVITY_STREAMS_NOTE
                                  select activityObj).SingleOrDefault();

            if (activityObject != null)
            {
                foreach (var enclosure in activityObject.Elements(buzzNS + XML_ATTACHMENT))
                {
                    var buzzEnclosure = ParseBuzzAttactment(enclosure);
                    if (buzzEnclosure != null)
                        feed.Enclosures.Add(buzzEnclosure);
                }
            }

            return feed;
        }

        public override Icon GetDefIcon(string downloadUrl)
        {
            return new Icon
            {
                DownloadUrl = Constants.RESX_GOOGLE_BUZZ_IMAGE_URI,
                Title = "Google Buzz Image",
                Description = "Feed Icon"
            };
        }

        public int ParseBuzzCount(XElement entry)
        {
            var buzzLikedCount = entry.Elements(atomNS + XML_LINK).Where(l => (string)l.Attribute(XML_REL) == NAMESPACE_GOOGLE_BUZZ_LIKED && (string)l.Attribute(XML_TYPE) == "application/poco+xml")
                                     .Select(l => l.Attribute(buzzNS + XML_COUNT))
                                     .SingleOrDefault();

            return buzzLikedCount != null ? (int)buzzLikedCount : 0;
        }

        private Enclosure ParseBuzzAttactment(XElement enclosureElement)
        {
            var enclosureLinks = enclosureElement.Elements(atomNS + XML_LINK)
                                .Where(enclosure => (string)enclosure.Attribute(XML_REL) == "enclosure");

            if (enclosureLinks.Count() == 0)
                enclosureLinks = enclosureElement.Elements(atomNS + XML_LINK)
                                .Where(enclosure => (string)enclosure.Attribute(XML_REL) == "alternate" && 
                                                    (string)enclosure.Attribute(XML_TYPE) == "application/x-shockwave-flash");

            foreach (var enclosureLink in enclosureLinks)
            {
                var enclosure = new Enclosure
                {
                    Url = GetMandatoryAttributeString(enclosureLink, XML_HREF),
                    Type = GetMandatoryAttributeString(enclosureLink, XML_TYPE),
                    Length = enclosureLink.Attribute(XML_LENGTH) != null ? (int)GetMandatoryAttribute(enclosureLink, XML_LENGTH) : 0
                };

                enclosure.Links.AddRange(from link in enclosureElement.Elements(atomNS + XML_LINK)
                                         select new Link
                                         {
                                             HRef = (string)link.Attribute(XML_HREF),
                                             LinkRel = ParseEnum<LinkRel>((string)link.Attribute(XML_REL), LinkRel.Other),
                                             Type = (string)link.Attribute(XML_TYPE)
                                         });
                return enclosure;
            }

            return null;
        }
    }
}
