/*/
 *
 * Author: Faraz Masood Khan
 * 
 * Date: Wednesday, August 27, 2008 1:01 PM
 * 
 * Class: FeedProcessor
 * 
 * Email: mk.faraz@gmail.com
 * 
 * Blogs: http://farazmasoodkhan.wordpress.com
 *
 * Website: http://www.linkedin.com/in/farazmasoodkhan
 *
 * Copyright: Faraz Masood Khan @ Copyright ©  2008
 *
/*/

using System;
using System.Linq;
using System.Xml.Linq;
using Feedbook.Model;
using System.Resources;
using System.Collections.Generic;
using Feedbook.Helper;
using CoreSystem.RefTypeExtension;
using CoreSystem.ValueTypeExtension;
using System.Globalization;

namespace Feedbook.Services.WebFeed
{
    internal class FeedProcessor
    {
        #region Constants

        #region DateTime Formats

        public const string DATE_FORMAT_RFC822 = "ddd, dd MMM yyyy HH:mm:ss zzz";
        public static readonly string[] DATE_FORMATS = new string[] { "r", "S", "U" };

        #endregion

        #region Default Values

        public const string DEF_LANGUAGE = "en-US";

        #endregion

        #region Namespaces

        public const string NAMESPACE_RSS1 = @"http://purl.org/rss/1.0/";
        public const string NAMESPACE_ATOM = @"http://www.w3.org/2005/Atom";
        public const string NAMESPACE_DUBLIN_CORE_1_1 = @"http://purl.org/dc/elements/1.1/";
        public const string NAMESPACE_DUBLIN_CORE = @"http://purl.org/dc/terms";
        public const string NAMESPACE_RDF = @"http://www.w3.org/1999/02/22-rdf-syntax-ns#";
        public const string NAMESPACE_CONTENT = @"http://purl.org/rss/1.0/modules/content/";
        public const string NAMESPACE_SYNDICATION = @"http://purl.org/rss/1.0/modules/syndication/";
        public const string NAMESPACE_PORTABLE_CONTACTS = "http://portablecontacts.net/ns/1.0";

        #region Activity Stream Namespaces

        public const string NAMESPACE_ACTIVITY_STREAMS = "http://activitystrea.ms/spec/1.0/";
        public const string NAMESPACE_ACTIVITY_STREAMS_NOTE = "http://activitystrea.ms/schema/1.0/note";
        public const string NAMESPACE_ACTIVITY_STREAMS_PHOTO = "http://activitystrea.ms/schema/1.0/photo";
        public const string NAMESPACE_ACTIVITY_STREAMS_VIDEO = "http://activitystrea.ms/schema/1.0/video";
        public const string NAMESPACE_ACTIVITY_STREAMS_ARTICLE = "http://activitystrea.ms/schema/1.0/article";
        public const string NAMESPACE_ACTIVITY_STREAMS_COMMENT = "http://activitystrea.ms/schema/1.0/comment";

        #endregion

        #endregion

        #region Common Tags

        public const string XML_ID = "id";
        public const string XML_URL = "url";
        public const string XML_ITEM = "item";
        public const string XML_LINK = "link";
        public const string XML_NAME = "name";
        public const string XML_TITLE = "title";
        public const string XML_IMAGE = "image";
        public const string XML_VERSION = "version";
        public const string XML_CHANNEL = "channel";
        public const string XML_TEXTINPUT = "textInput";
        public const string XML_DESCRIPTION = "description";
        public const string XML_LENGTH = "length";
        public const string XML_TYPE = "type";
        public const string XML_AUTHOR = "author";
        public const string XML_CATEGORY = "category";
        public const string XML_GENERATOR = "generator";
        public const string XML_SOURCE = "source";
        public const string XML_LANGUAGE = "language";
        public const string XML_CONTRIBUTOR = "contributor";
        public const string XML_RIGHTS = "rights";
        public const string XML_FORMAT = "format";

        #endregion

        #region RSS Tags

        public const string XML_RSS = "rss";
        public const string XML_DAY = "day";
        public const string XML_HOUR = "hour";
        public const string XML_WIDTH = "width";
        public const string XML_HEIGHT = "height";
        public const string XML_RATING = "rating";
        public const string XML_PUBDATE = "pubDate";
        public const string XML_SKIPDAYS = "skipDays";
        public const string XML_COPYRIGHT = "copyright";
        public const string XML_SKIPHOURS = "skipHours";
        public const string XML_WEBMASTER = "webMaster";
        public const string XML_LASTBUILDDATE = "lastBuildDate";
        public const string XML_MANAGINGEDITOR = "managingEditor";
        public const string XML_DOMAIN = "domain";
        public const string XML_DOCS = "docs";
        public const string XML_CLOUD = "cloud";
        public const string XML_TTL = "ttl";
        public const string XML_COMMENTS = "comments";
        public const string XML_ENCLOSURE = "enclosure";
        public const string XML_GUID = "guid";
        public const string XML_ISPERMALINK = "isPermaLink";
        public const string XML_EXPIRATIONDATE = "expirationDate";

        #endregion

        #region RDF Tags

        public const string XML_RDF = "RDF";
        public const string XML_RESOURCE = "resource";
        public const string XML_ABOUT = "about";
        public const string XML_ITEMS = "items";

        #endregion

        #region ATOM Tags

        public const string XML_FEED = "feed";
        public const string XML_EMAIL = "email";
        public const string XML_URI = "uri";
        public const string XML_TERM = "term";
        public const string XML_SCHEME = "scheme";
        public const string XML_LABEL = "label";
        public const string XML_ICON = "icon";
        public const string XML_HREF = "href";
        public const string XML_REL = "rel";
        public const string XML_HREFLANG = "hreflang";
        public const string XML_LOGO = "logo";
        public const string XML_SUBTITLE = "subtitle";
        public const string XML_UPDATED = "updated";
        public const string XML_ENTRY = "entry";
        public const string XML_CONTENT = "content";
        public const string XML_SRC = "src";
        public const string XML_PUBLISHED = "published";
        public const string XML_SUMMARY = "summary";

        #endregion

        #region Dublin Core Extension Tags

        // Not all tags are defined here
        public const string XML_COVERAGE = "coverage";
        public const string XML_RELATION = "relation";
        public const string XML_SUBJECT = "subject";
        public const string XML_CREATOR = "creator";
        public const string XML_PUBLISHER = "publisher";
        public const string XML_DATE = "date";
        public const string XML_IDENTIFIER = "identifier";

        #endregion

        #region Content Extension Tags

        public const string XML_ENCODING = "encoding";
        public const string XML_ENCODED = "encoded";

        #endregion

        #region Syndication Extension Tags

        public const string XML_UPDATEPERIOD = "updatePeriod"; // ( 'hourly' | 'daily' | 'weekly' | 'monthly' | 'yearly' )
        public const string XML_UPDATEFREQUENCY = "updateFrequency"; // ( a positive integer )
        public const string XML_UPDATEBASE = "updateBase"; // ( #PCDATA ) [W3CDTF]

        #endregion

        #region Portable Contacts Tags

        public const string XML_PHOTOURL = "photoUrl";

        #endregion

        #region Activity Stream Tags

        public const string XML_OBJECT = "object";
        public const string XML_OBJECT_TYPE = "object-type";

        #endregion

        #endregion

        public static readonly XNamespace rdfNS = XNamespace.Get(NAMESPACE_RDF);
        public static readonly XNamespace rss1NS = XNamespace.Get(NAMESPACE_RSS1);
        public static readonly XNamespace atomNS = XNamespace.Get(NAMESPACE_ATOM);

        public static readonly XNamespace coNS = XNamespace.Get(NAMESPACE_CONTENT);
        public static readonly XNamespace dcNS = XNamespace.Get(NAMESPACE_DUBLIN_CORE);
        public static readonly XNamespace dc11NS = XNamespace.Get(NAMESPACE_DUBLIN_CORE_1_1);
        public static readonly XNamespace syNS = XNamespace.Get(NAMESPACE_SYNDICATION);
        public static readonly XNamespace pocoNS = XNamespace.Get(NAMESPACE_PORTABLE_CONTACTS);
        public static readonly XNamespace activityNS = XNamespace.Get(NAMESPACE_ACTIVITY_STREAMS);


        public virtual Channel Parse(string xmlString, string downloadUrl)
        {
            XElement feedElement;
            try
            {
                feedElement = XElement.Parse(xmlString, LoadOptions.PreserveWhitespace);
            }
            catch (Exception excep)
            {
                throw new InvalidOperationException("Xml is ill formed", excep);
            }

            try
            {
                switch (feedElement.Name.LocalName)
                {
                    case XML_RSS:
                        return ParseRSS(feedElement, downloadUrl);
                    case XML_RDF:
                        return ParseRDF(feedElement, downloadUrl);
                    case XML_FEED:
                        return ParseAtom(feedElement, downloadUrl);
                    default:
                        throw new InvalidOperationException(string.Format("Cannot recognized feed type: {0}", feedElement.Name.LocalName));
                }
            }
            catch (Exception excep)
            {
                throw new InvalidOperationException("Error occurred while parsing feed", excep);
            }

        }

        public virtual Channel ParseRSS(XElement rssElement, string downloadUrl)
        {
            XElement channelElement;
            if ((channelElement = rssElement.Element(XML_CHANNEL)) == null)
                throw new InvalidOperationException("<channel> tag is missing in feed");

            var channel = new Channel
            {
                DownloadUrl = downloadUrl,

                FeedType = GetRSSTypeVersion(rssElement),                

                Title = CleanText((string)channelElement.Element(XML_TITLE)),

                Description = (string)channelElement.Element(XML_DESCRIPTION),

                Language = (string)channelElement.Element(XML_LANGUAGE),

                Copyrights = (string)channelElement.Element(XML_COPYRIGHT),

                Generator = (string)channelElement.Element(XML_GENERATOR),

                Icon = GetImage(XNamespace.None, channelElement, GetDefIcon(downloadUrl)),

                Updated = ParseDateTime(channelElement, XML_LASTBUILDDATE, DateTime.Now),

                Published = ParseDateTime(channelElement, XML_PUBDATE, DateTime.Now),

                Ttl = channelElement.Element(XML_TTL) != null ? (int)channelElement.Element(XML_TTL) : ComputeTTLfromSyndication(channelElement, 0)
            };

            // Skip hours are always represented in GMT time zone and range from 0 to 23
            channel.SkipHours.AddRange(from skipHours in channelElement.Elements(XML_SKIPHOURS)
                                       from hour in skipHours.Elements(XML_HOUR)
                                       select (int)hour);

            // Skip days are week days Monday, Tuesday, Wednesday, Thursday, Friday, Saturday or Sunday. 
            // However it is not mentioned that skip days are also represented in GMT
            channel.SkipDays.AddRange(from skipDays in channelElement.Elements(XML_SKIPDAYS)
                                      from day in skipDays.Elements(XML_DAY)
                                      select GetDayOfWeek(day));

            channel.People.AddRange(from person in channelElement.Elements()
                                    where person.Name.LocalName.In(XML_MANAGINGEDITOR, XML_WEBMASTER)
                                    select GetRSSPerson(person));

            channel.Links.AddRange(from link in channelElement.Elements(atomNS + XML_LINK)
                                   where Uri.IsWellFormedUriString((string)link.Attribute(XML_HREF), UriKind.Absolute)
                                   select new Link
                                   {
                                       HRef = (string)link.Attribute(XML_HREF),
                                       LinkRel = ParseEnum<LinkRel>((string)link.Attribute(XML_REL), LinkRel.Alternate),
                                       Type = (string)link.Attribute(XML_TYPE)
                                   });

            channel.Links.AddRange(from link in channelElement.Elements(XML_LINK)
                                   where Uri.IsWellFormedUriString((string)link, UriKind.Absolute)
                                   select new Link
                                   {
                                       HRef = (string)link,
                                       LinkRel = LinkRel.Alternate,
                                       Title = channel.Title
                                   });

            channel.Categories.AddRange(from category in channelElement.Elements(XML_CATEGORY)
                                        select new Category
                                        {
                                            Name = (string)category,
                                            Domain = (string)category.Attribute(XML_DOMAIN)
                                        });

            if (channel.Links.Count(l => l.LinkRel == LinkRel.Self) == 0)
            {
                Link selfLink = new Link
                {
                    HRef = downloadUrl,
                    LinkRel = LinkRel.Self,
                    Title = channel.Title
                };

                if (channel.Links.Count(l => Link.IsLinkEqual(l, selfLink)) > 0)
                    channel.Links.Remove(channel.Links.Single(l => Link.IsLinkEqual(l, selfLink)));

                channel.Links.Add(selfLink);
            }

            if (channel.Published.Equals(DateTime.MinValue))
                channel.Published = ParseDateTime(channelElement, dc11NS + XML_DATE, DateTime.Now);

            if (channel.Updated.Equals(DateTime.MinValue))
                channel.Updated = channel.Published;

            channel.Feeds.AddRange(from item in channelElement.Elements(XML_ITEM)
                                   select ParseItem(XNamespace.None, item, channel.Updated));

            channel.ChannelId = channel.DownloadUrl;

            return channel;
        }

        public virtual Channel ParseRDF(XElement rdfElement, string downloadUrl)
        {
            XElement channelElement;

            if ((channelElement = rdfElement.Element(rss1NS + XML_CHANNEL)) == null)
                throw new InvalidOperationException("<channel> tag is missing in the feed");

            var channel = new Channel
            {
                DownloadUrl = downloadUrl,

                FeedType = FeedType.RSS_100,

                Language = DEF_LANGUAGE,

                Ttl = ComputeTTLfromSyndication(channelElement, 0),

                Title = CleanText((string)channelElement.Element(rss1NS + XML_TITLE)),

                Description = (string)channelElement.Element(rss1NS + XML_DESCRIPTION),

                Updated = ParseDateTime(channelElement, dc11NS + XML_DATE, DateTime.Now),

                Published = ParseDateTime(channelElement, dc11NS + XML_DATE, DateTime.Now),

                Icon = GetImage(rss1NS, rdfElement, null) ?? GetImage(rdfNS, channelElement, GetDefIcon(downloadUrl))
            };

            channel.Links.AddRange(from link in channelElement.Attributes(rdfNS + XML_ABOUT)
                                   where Uri.IsWellFormedUriString((string)link, UriKind.Absolute)
                                   select new Link
                                   {
                                       HRef = (string)link,
                                       LinkRel = LinkRel.Self,
                                       Title = channel.Title
                                   });

            channel.Links.AddRange(from link in channelElement.Elements(rss1NS + XML_LINK)
                                   where Uri.IsWellFormedUriString((string)link, UriKind.Absolute)
                                   select new Link
                                   {
                                       HRef = (string)link,
                                       LinkRel = LinkRel.Alternate,
                                       Title = channel.Title
                                   });

            channel.People.AddRange(from creator in channelElement.Elements(dc11NS + XML_CREATOR)
                                    select new Person
                                    {
                                        Name = (string)creator,
                                        PersonRole = PersonRole.Author
                                    });

            channel.Feeds.AddRange(from item in rdfElement.Elements(rss1NS + XML_ITEM)
                                   select ParseItem(rss1NS, item, channel.Updated));

            channel.ChannelId = channel.DownloadUrl;

            return channel;
        }

        public virtual Channel ParseAtom(XElement feedElement, string downloadUrl)
        {

            var channel = new Channel
            {
                DownloadUrl = downloadUrl,

                FeedType = FeedType.Atom_100,

                ChannelId = GetMandatoryElementString(feedElement, atomNS + XML_ID),

                Title = ParseTextConstruct(GetMandatoryElement(feedElement, atomNS + XML_TITLE)),

                Updated = ParseDateTime((string)feedElement.Element(atomNS + XML_UPDATED), DateTime.Now),

                Published = ParseDateTime((string)feedElement.Element(atomNS + XML_UPDATED), DateTime.Now),

                Generator = (string)feedElement.Element(atomNS + XML_GENERATOR),

                Copyrights = (string)feedElement.Element(atomNS + XML_RIGHTS),

                Ttl = ComputeTTLfromSyndication(feedElement, 0),

                Language = DEF_LANGUAGE,

                Icon = new Icon { DownloadUrl = (string)feedElement.Element(atomNS + XML_ICON), Title = GetMandatoryElementString(feedElement, atomNS + XML_TITLE) }
            };

            if (string.IsNullOrEmpty(channel.Icon.DownloadUrl))
                channel.Icon.DownloadUrl = (string)feedElement.Element(atomNS + XML_LOGO);

            if (string.IsNullOrEmpty(channel.Icon.DownloadUrl))
                channel.Icon = GetDefIcon(downloadUrl);

            channel.Links.AddRange(from link in feedElement.Elements(atomNS + XML_LINK)
                                   where Uri.IsWellFormedUriString((string)link.Attribute(XML_HREF), UriKind.Absolute)
                                   select GetLink(link));

            channel.Categories.AddRange(from category in feedElement.Elements(atomNS + XML_CATEGORY)
                                        select new Category
                                        {
                                            Name = GetMandatoryAttributeString(category, XML_TERM),
                                            Domain = (string)category.Attribute(XML_SCHEME)
                                        });

            channel.People.AddRange(from person in feedElement.Elements(atomNS + XML_AUTHOR)
                                    select ParsePersonConstruct(person));

            channel.People.AddRange(from person in feedElement.Elements(atomNS + XML_CONTRIBUTOR)
                                    select ParsePersonConstruct(person));

            channel.Feeds.AddRange(from entry in feedElement.Elements(atomNS + XML_ENTRY)
                                   select ParseEntry(entry, channel.People));

            return channel;
        }

        public virtual Feed ParseItem(XNamespace ns, XElement item, DateTime updated)
        {
            XElement guid;
            XElement link;

            var feed = new Feed
            {
                Title = CleanText((string)item.Element(ns + XML_TITLE)),

                CommentUrl = (string)item.Element(XML_COMMENTS),

                Updated = ParseDateTime(item, XML_PUBDATE, ParseDateTime(item, dc11NS + XML_DATE, updated))
            };


            // Finding link with rel="self" else using any link
            link = item.Elements().FirstOrDefault(e => e.Name == (ns + XML_LINK) && (string)e.Attribute(XML_REL) == "self")
                   ?? item.Element(ns + XML_LINK);

            // Handling Link
            if (link != null)
                feed.Link = new Link
                {
                    HRef = (string)item.Element(ns + XML_LINK),
                    LinkRel = LinkRel.Self,
                    Title = feed.Title
                };

            // Finding guid of the post
            guid = item.Element(XML_GUID);
            if (guid != null)
            {
                feed.Guid = (string)guid;
                if (guid.Attribute(XML_ISPERMALINK) != null && (bool)guid.Attribute(XML_ISPERMALINK))
                    feed.Link = new Link
                    {
                        HRef = (string)guid,
                        LinkRel = LinkRel.Self,
                        Title = feed.Title
                    };
            }
            else
            {
                // Using self link as guid
                if (feed.Link == null)
                    throw new InvalidOperationException(string.Format("Feed '{0}' doesn't contains any link to it's orginal post", feed.Title));
                feed.Guid = feed.Link.HRef;
            }

            // Parsing all descriptions of feed with description tag (default RSS tag)          
            String contentType;

            feed.Descriptions.AddRange(from description in item.Elements(ns + XML_DESCRIPTION)
                                       select new Content
                                       {
                                           Type = (contentType = GetContentType(ns, description)),
                                           Value = CleanContent(contentType, (string)description)
                                       });

            // Parsing all xhtml descriptions define using Content Extension
            feed.Descriptions.AddRange(from encoded in item.Elements(coNS + XML_ENCODED)
                                       select new Content
                                       {
                                           Value = CleanXML((string)encoded),
                                           Type = Content.XHTML
                                       });

            // Parsing all enclosures
            feed.Enclosures.AddRange(from enclosure in item.Elements(XML_ENCLOSURE)
                                     select new Enclosure
                                     {
                                         Feed = feed,
                                         Url = (string)enclosure.Attribute(XML_URL),
                                         Type = (string)enclosure.Attribute(XML_TYPE),
                                         Length = GetInt(enclosure.Attribute(XML_LENGTH), 0)
                                     });

            // Parsing person from RSS person construct
            feed.People.AddRange(from element in item.Elements()
                                 where element.Name.LocalName.In(XML_AUTHOR, XML_CREATOR)
                                 select GetRSSPerson(element));

            // Parsing categories ...
            feed.Categories.AddRange(from category in item.Elements(XML_CATEGORY)
                                     select new Category
                                     {
                                         Name = CleanText((string)category),
                                         Domain = (string)category.Attribute(XML_DOMAIN)
                                     });
            return feed;
        }

        public virtual Feed ParseEntry(XElement entry, IEnumerable<Person> people)
        {
            var feed = new Feed
            {
                Guid = GetMandatoryElementString(entry, atomNS + XML_ID),

                Title = CleanText(ParseTextConstruct(GetMandatoryElement(entry, atomNS + XML_TITLE))),

                Updated = ParseDateTime((string)entry.Element(atomNS + XML_UPDATED),
                          ParseDateTime((string)entry.Element(atomNS + XML_PUBLISHED), DateTime.Now))
            };

            // Parsing atom:category
            feed.Categories.AddRange(from category in entry.Elements(atomNS + XML_CATEGORY)
                                     select new Category
                                     {
                                         Name = GetMandatoryAttributeString(category, XML_TERM),
                                         Domain = (string)category.Attribute(XML_SCHEME)
                                     });


            // Parsing atom:author
            feed.People.AddRange(from person in entry.Elements(atomNS + XML_AUTHOR)
                                 select ParsePersonConstruct(person));

            //Parsing atom:contributor
            feed.People.AddRange(from person in entry.Elements(atomNS + XML_CONTRIBUTOR)
                                 select ParsePersonConstruct(person));

            feed.Descriptions.AddRange(from content in entry.Elements(atomNS + XML_CONTENT)
                                       select ParseTextConstruct(content));

            feed.Descriptions.AddRange(from summary in entry.Elements(atomNS + XML_SUMMARY)
                                       select ParseTextConstruct(summary));

            feed.Enclosures.AddRange(from enclosure in entry.Elements(atomNS + XML_LINK)
                                     where (string)enclosure.Attribute(XML_REL) == "enclosure"
                                     select new Enclosure
                                     {
                                         Feed = feed,
                                         Url = GetMandatoryAttributeString(enclosure, XML_HREF),
                                         Type = GetMandatoryAttributeString(enclosure, XML_TYPE),
                                         Length = GetInt(enclosure.Attribute(XML_LENGTH), 0)
                                     });

            if (feed.People.Count == 0 && people != null)
                feed.People.AddRange(people);

            var selfLinks = from link in entry.Elements(atomNS + XML_LINK)
                            where (string)link.Attribute(XML_REL) == "self" && (string)link.Attribute(XML_TYPE) != "application/atom+xml"
                            select link;

            if (selfLinks.Count() > 0)
                feed.Link = GetLink(selfLinks.First());
            else
            {
                if (feed.Guid != null && feed.Guid.StartsWith("http")
                    && Uri.IsWellFormedUriString(feed.Guid, UriKind.Absolute))
                    feed.Link = new Link
                    {
                        HRef = feed.Guid,
                        Title = feed.Title,
                        LinkRel = LinkRel.Self
                    };
                else
                {
                    var alternateLinks = from link in entry.Elements(atomNS + XML_LINK)
                                         where (string)link.Attribute(XML_REL) == "alternate"
                                               && (string)link.Attribute(XML_TYPE) == "text/html"
                                         select link;

                    if (alternateLinks.Count() == 0)
                        alternateLinks = entry.Elements(atomNS + XML_LINK);

                    if (alternateLinks.Count() == 0)
                        throw new InvalidOperationException("Not link release to feed found in feed entry");

                    feed.Link = GetLink(alternateLinks.First());
                }

            }

            return feed;
        }

        #region Helper Functions

        public static int GetInt(XAttribute attribute, int defValue)
        {
            if (attribute != null)
                int.TryParse((string)attribute, out defValue);

            return defValue;
        }

        public static XElement GetMandatoryElement(XElement element, XName name)
        {
            XElement retElement = element.Element(name);
            if (retElement != null)
                return retElement;
            throw new InvalidOperationException(string.Format("Mandatory element: {0} is missing", name));
        }

        public static XAttribute GetMandatoryAttribute(XElement element, XName name)
        {
            XAttribute retAttribute = element.Attribute(name);
            if (retAttribute != null)
                return retAttribute;
            throw new InvalidOperationException(string.Format("Mandatory attribute: {0} is missing from element: {1}", name, element.Name));
        }

        public static string GetMandatoryElementString(XElement element, XName name)
        {
            return (string)GetMandatoryElement(element, name);
        }

        public static string GetMandatoryAttributeString(XElement element, XName name)
        {
            return (string)GetMandatoryAttribute(element, name);
        }

        public static DayOfWeek GetDayOfWeek(XElement day)
        {
            try
            {
                return ((string)day).ToEnum<DayOfWeek>();
            }
            catch (Exception ex)
            {
                throw new IndexOutOfRangeException(string.Format("Unable to parse value: '{0}' as week days", (string)day), ex);
            }
        }

        public static FeedType GetRSSTypeVersion(XElement rssElement)
        {
            XAttribute rssVersion = rssElement.Attribute(XML_VERSION);
            if (rssVersion == null)
                throw new InvalidOperationException("RSS version attribute is missing");

            switch ((string)rssVersion)
            {
                case "0.91":
                    return FeedType.RSS_091;
                case "0.92":
                    return FeedType.RSS_092;
                case "0.93":
                    return FeedType.RSS_093;
                case "0.94":
                    return FeedType.RSS_094;
                case "2.0":
                    return FeedType.RSS_200;
                case "2.0.1":
                    return FeedType.RSS_201;
                default:
                    throw new InvalidOperationException(string.Format("Invalid RSS feed version: '{0}'", (string)rssVersion));
            }
        }

        public static DateTime ParseDateTime(XElement parentElement, XName dateElementName, DateTime defValue)
        {
            return ParseDateTime((string)parentElement.Element(dateElementName), defValue);
        }

        public static DateTime ParseDateTime(string dateString, DateTime defValue)
        {
            if (string.IsNullOrEmpty(dateString))
                return defValue.ToLocalTime();

            try
            {
                // Parse the dates using the standard
                // universal date format
                // 2009-04-26T03:22:14.0631217+05:00
                // 2009-04-26T03:22:14.0631217Z
                // 2009-04-26T03:22:14.0631217 With no time zone than assume universal
                return DateTime.Parse(dateString,
                     CultureInfo.InvariantCulture,
                     DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal).ToLocalTime();
            }
            catch
            {
                try
                {
                    // Standard formats failed, try the "r" "S"
                    // and "U" formats

                    return DateTime.ParseExact(dateString, DATE_FORMATS,
                              DateTimeFormatInfo.InvariantInfo,
                              DateTimeStyles.AdjustToUniversal).ToLocalTime();
                }
                catch
                {
                    try
                    {
                        // All the standards formats have failed,
                        //try the dreaded RFC822 format

                        //return DateTime.ParseExact(dateString, DATE_FORMAT_RFC822,
                        //        DateTimeFormatInfo.InvariantInfo,
                        //        DateTimeStyles.AdjustToUniversal);
                        return RFC822(dateString).ToLocalTime();
                    }
                    catch
                    {

                        return defValue.ToLocalTime();
                    }
                }
            }
        }

        public static int ComputeTTLfromSyndication(XElement element, int defValue)
        {
            string updatePeriod = (string)element.Element(syNS + XML_UPDATEPERIOD);
            int? updateFrequency = (int?)element.Element(syNS + XML_UPDATEFREQUENCY);

            if (string.IsNullOrEmpty(updatePeriod)
                && !updateFrequency.HasValue)
                return defValue;

            updateFrequency = updateFrequency.HasValue ? updateFrequency.Value : 1;

            switch (updatePeriod)
            {
                case "hourly":
                    return 60 / updateFrequency.Value;
                default: //"daily"
                    return (60 * 24) / updateFrequency.Value;
                case "weekly":
                    return (60 * 24 * 7) / updateFrequency.Value;
                case "monthly":
                    return (60 * 24 * 7 * 30) / updateFrequency.Value;
                case "yearly":
                    return (60 * 24 * 7 * 30 * 12) / updateFrequency.Value;
            }
        }

        public static Person GetRSSPerson(XElement xperson)
        {
            Person person;
            Func<string, Person> transform = value =>
            {
                int index = value.IndexOf(' ');
                return (index != -1)
                           ? new Person
                           {
                               Email = value.Substring(0, index),
                               Name = value.Substring(index).Trim(' ', '(', ')')
                           }
                           : new Person
                           {
                               Email = value
                           };
            };

            switch (xperson.Name.LocalName)
            {
                case XML_WEBMASTER:
                    person = transform((string)xperson);
                    person.PersonRole = PersonRole.WebMaster;
                    person.Name = person.Name ?? person.PersonRole.ToDescription();
                    return person;
                case XML_MANAGINGEDITOR:
                    person = transform((string)xperson);
                    person.PersonRole = PersonRole.ManagingEditor;
                    person.Name = person.Name ?? person.PersonRole.ToDescription();
                    return person;
                case XML_AUTHOR:
                    person = transform((string)xperson);
                    person.PersonRole = PersonRole.Author;
                    person.Name = person.Name ?? person.PersonRole.ToDescription();
                    return person;
                case XML_CREATOR:
                    return new Person
                    {
                        Name = (string)xperson,
                        PersonRole = PersonRole.Author
                    };
                default:
                    throw new InvalidOperationException(string.Format("Cannot able to create person construct from: '{0}'", xperson.Name));
            }
        }

        public static Icon GetImage(XNamespace ns, XElement imageParent, Icon defValue)
        {

            XElement image = imageParent.Element(ns + XML_IMAGE);
            if (image != null)
            {
                try
                {
                    Icon retVal = image.Attribute(rdfNS + XML_RESOURCE) != null ?
                        // If RDF resource attribute found then feed is RSS 1.0 with RDF support,
                        // so this attribute's value should used to as download url for image
                                                                  new Icon
                                                                  {
                                                                      DownloadUrl = (string)image.Attribute(rdfNS + XML_RESOURCE),
                                                                      Title = (string)imageParent.Element(ns + XML_TITLE),
                                                                      Link = (string)imageParent.Element(ns + XML_LINK)
                                                                  }

                        // If resource attribute not found than feed must be RSS 0.92, 0.93 or 2.0,
                        // url element should be used as download link to image file. Only image type supported 
                        // by RSS 2.0 are JIF, JPEG and PNG. Width and Height are optional elements and their values are 
                        // in pixels. Width can range up to 144 pixel and default is 88 pixel, Height can range up to
                        // 400 pixel and default is 31 
                                                                  : new Icon
                                                                  {
                                                                      DownloadUrl = (string)image.Element(ns + XML_URL),
                                                                      Link = (string)image.Element(ns + XML_LINK),
                                                                      Title = (string)image.Element(ns + XML_TITLE),
                                                                      Description = (string)image.Element(ns + XML_DESCRIPTION),
                                                                      Width = (image.Element(ns + XML_WIDTH) != null ? (int)image.Element(XML_WIDTH) : 88),
                                                                      Height = (image.Element(ns + XML_HEIGHT) != null ? (int)image.Element(XML_HEIGHT) : 31)
                                                                  };

                    //retVal.Image = Downloader.DownloadBytes(retVal.DownloadUrl);
                    return retVal;
                }
                catch { }
            }

            return defValue;
        }

        public static T ParseEnum<T>(string stringValue, T defValue) where T : struct
        {
            try
            {
                return (T)Enum.Parse(typeof(T), stringValue, true);
            }
            catch
            {
                return defValue;
            }
        }

        public static Link GetLink(XElement link)
        {
            return new Link
            {
                Title = (string)link.Attribute(XML_TITLE),

                HRef = GetMandatoryAttributeString(link, XML_HREF),

                LinkRel = ParseEnum<LinkRel>((string)link.Attribute(XML_REL), LinkRel.Alternate),

                Type = (string)link.Attribute(XML_TYPE)
            };
        }

        public static string GetContentType(XNamespace ns, XElement content)
        {
            if (content.Attribute(ns + XML_TYPE) != null)
                return (string)content.Attribute(ns + XML_TYPE);

            string contentString = (string)content;

            return Content.DetermineType(contentString);
        }

        public virtual Icon GetDefIcon(string downloadUrl)
        {
            return new Icon
            {
                DownloadUrl = Constants.RESX_RSS_IMAGE_URI,
                Title = "RSS Image",
                Description = "Feed Icon"
            };
        }

        #region Construct Functions

        public static Person ParsePersonConstruct(XElement person)
        {
            return new Person
            {
                Name = GetMandatoryElementString(person, atomNS + XML_NAME),

                UserId = (string)person.Element(pocoNS + XML_ID),

                ImageUrl = (string)person.Element(pocoNS + XML_PHOTOURL),

                Email = (string)person.Element(atomNS + XML_EMAIL),

                Url = (string)person.Element(atomNS + XML_URI),

                PersonRole = ParseEnum<PersonRole>(person.Name.LocalName, PersonRole.Author)
            };
        }

        public static Content ParseTextConstruct(XElement textElement)
        {
            var type = (string)textElement.Attribute(XML_TYPE);

            switch (type)
            {
                case Content.HTML:
                    return new Content
                    {
                        Value = CleanXML((string)textElement),
                        Type = Content.XHTML
                    };
                case null:
                    return new Content
                    {
                        Value = CleanText((string)textElement),
                        Type = Content.TEXT
                    };
                default:
                    return new Content
                    {
                        Value = CleanContent(type, ((string)textElement)),
                        Type = type
                    };
            }
        }

        #endregion

        #endregion

        private static readonly string junkApos = System.Text.Encoding.UTF8.GetString(new byte[] { /*0x20, 0x00, 0x00, 0x00,*/ 0xe2, 0x00, 0x00, 0x00, 0xac, 0x20, 0x00, 0x00, 0xdc, 0x02, 0x00, 0x00 }, 0, 12);

        public static string CleanContent(string contentType, string content)
        {
            if (contentType == Content.TEXT)
                return CleanText(content);

            return CleanXML(content);
        }

        public static string CleanText(string text)
        {
            if (text != null)
            {
                return text.Trim('\n', '\r')
                           .Replace("â€œ", "\"")
                           .Replace("â€", "\"")
                           .Replace("â€™", "'")
                           .Replace("â€“", "-")
                           .Replace(junkApos, "'")
                           .Replace("â€¦", "…")
                           .Replace("Â", "&")
                           .Replace("â€”", "—")
                           .Replace("â€¢", "•");
            }

            return text;
        }

        public static string CleanXML(string xml)
        {
            if (xml != null)
            {
                return xml.Trim('\n', '\r')
                           .Replace("â€œ", "&quot;")
                           .Replace("â€", "&quot;")
                           .Replace("â€™", "&apos;")
                           .Replace("â€“", "-")
                           .Replace("â€¦", "…")
                           .Replace(junkApos, "&apos;")
                           .Replace("Â", "&amp;")
                           .Replace("â€”", "—")
                           .Replace("â€¢", "•");
            }

            return xml;
        }

        public static DateTime RFC822(string dateString)
        {
            System.DateTime date;
            int pos = dateString.LastIndexOf(" ") + 1;
            string timeZone = dateString.Substring(pos).Trim();

            try
            {
                date = Convert.ToDateTime(dateString);
                if (timeZone == "Z")
                    return date.ToUniversalTime();
                else if (timeZone == "GMT")
                    return date.ToUniversalTime();
            }
            catch (Exception)
            {
                //System.Diagnostics.Trace.WriteLine(x.Message);
            }

            date = DateTime.ParseExact(dateString.Substring(0, pos - 1), "ddd, dd MMM yyyy HH:mm:ss", DateTimeFormatInfo.InvariantInfo);

            if (dateString[pos] == '+')
            {
                int h = Convert.ToInt32(dateString.Substring(pos + 2, 2));
                date = date.AddHours(-h);
                int m = Convert.ToInt32(dateString.Substring(pos + 4, 2));
                date = date.AddMinutes(-m);
            }
            else if (dateString[pos] == '-')
            {
                int h = Convert.ToInt32(dateString.Substring(pos + 2, 2));
                date = date.AddHours(h);
                int m = Convert.ToInt32(dateString.Substring(pos + 4, 2));
                date = date.AddMinutes(m);
            }
            else
            {
                switch (timeZone)
                {
                    case "A":
                        return date.AddHours(1);
                    case "B":
                        return date.AddHours(2);
                    case "C":
                        return date.AddHours(3);
                    case "D":
                        return date.AddHours(4);
                    case "E":
                        return date.AddHours(5);
                    case "F":
                        return date.AddHours(6);
                    case "G":
                        return date.AddHours(7);
                    case "H":
                        return date.AddHours(8);
                    case "I":
                        return date.AddHours(9);
                    case "K":
                        return date.AddHours(10);
                    case "L":
                        return date.AddHours(11);
                    case "M":
                        return date.AddHours(12);
                    case "N":
                        return date.AddHours(-1);
                    case "O":
                        return date.AddHours(-2);
                    case "P":
                        return date.AddHours(-3);
                    case "Q":
                        return date.AddHours(-4);
                    case "R":
                        return date.AddHours(-5);
                    case "S":
                        return date.AddHours(-6);
                    case "T":
                        return date.AddHours(-7);
                    case "U":
                        return date.AddHours(-8);
                    case "V":
                        return date.AddHours(-9);
                    case "W":
                        return date.AddHours(-10);
                    case "X":
                        return date.AddHours(-11);
                    case "Y":
                        return date.AddHours(-12);
                    case "EST":
                        return date.AddHours(5);
                    case "EDT":
                        return date.AddHours(4);
                    case "CST":
                        return date.AddHours(6);
                    case "CDT":
                        return date.AddHours(5);
                    case "MST":
                        return date.AddHours(7);
                    case "MDT":
                        return date.AddHours(6);
                    case "PST":
                        return date.AddHours(8);
                    case "PDT":
                        return date.AddHours(7);
                    case "PKT":
                        return date.AddHours(-5);
                    case "BST":
                        return date.AddHours(-1);
                }
            }

            return date;
        }
    }
}