using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Feedbook.Model;
using Feedbook.Helper;
using System.Xml.Linq;
using Feedbook.Services.WebFeed;
using CoreSystem.RefTypeExtension;

namespace Feedbook.Services.Google.GData
{
    internal class GService
    {
        #region Service Urls

        public const string BOOK_SEARCH_SERVICE = "http://books.google.com/books/feeds/volumes";

        public const string SIDEWIKI_SERVICE = "http://www.google.com/sidewiki/feeds/entries";

        #endregion

        public static Feed[] GetFeeds(GQuery query)
        {
            string atomFeed = Downloader.DownloadString(query.ToString());
            Channel channel = (new FeedProcessor()).Parse(atomFeed, query.ToString());

            if (channel != null && query.ServiceUrl != null)
            {
                if (query.ServiceUrl.StartsWith(BOOK_SEARCH_SERVICE))
                    BookSearchFeeds(channel, atomFeed);
                else if (query.ServiceUrl.StartsWith(SIDEWIKI_SERVICE))
                    SidewikiFeeds(channel, atomFeed);
            }

            return (channel != null) ? channel.Feeds.ToArray() : null;
        }

        private static void BookSearchFeeds(Channel channel, string atomFeed)
        {
            XElement xChannel = XElement.Parse(atomFeed);
            //Book Search API uses dublin core for book information so fixing feeds

            foreach (Feed feed in channel.Feeds)
            {
                XElement xFeed = (from entry in xChannel.Elements(FeedProcessor.atomNS + FeedProcessor.XML_ENTRY)
                                  where feed.Link != null && feed.Link.HRef == (string)entry.Element(FeedProcessor.atomNS + FeedProcessor.XML_ID)
                                  select entry).FirstOrDefault();

                if (xFeed == null)
                    continue;

                XElement xLink = (from link in xFeed.Elements(FeedProcessor.atomNS + FeedProcessor.XML_LINK)
                                  where (string)link.Attribute(FeedProcessor.XML_REL) == "alternate"
                                        && (link.Attribute(FeedProcessor.XML_TYPE) == null
                                            || (string)link.Attribute(FeedProcessor.XML_TYPE) == "text/html")
                                  select link).FirstOrDefault();

                if (xLink != null)
                    feed.Link = new Link
                    {
                        LinkRel = LinkRel.Alternate,
                        Title = feed.Title,
                        HRef = (string)xLink.Attribute(FeedProcessor.XML_HREF),
                        Type = (string)xLink.Attribute(FeedProcessor.XML_TYPE)
                    };

                feed.People.Clear();
                feed.People.AddRange(from author in xFeed.Elements(FeedProcessor.dcNS + FeedProcessor.XML_CREATOR)
                                     select FeedProcessor.GetRSSPerson(author));

                feed.Categories.Clear();
                feed.Categories.AddRange(from category in xFeed.Elements(FeedProcessor.dcNS + FeedProcessor.XML_SUBJECT)
                                         select new Category { Name = (string)category });

                feed.Descriptions.Clear();
                string contentType;
                feed.Descriptions.AddRange(from description in xFeed.Elements(FeedProcessor.dcNS + FeedProcessor.XML_DESCRIPTION)
                                           select new Content
                                           {
                                               Type = (contentType = FeedProcessor.GetContentType(FeedProcessor.dcNS, description)),
                                               Value = FeedProcessor.CleanContent(contentType,(string)description)
                                               
                                           });

                XElement xDate = xFeed.Element(FeedProcessor.dcNS + FeedProcessor.XML_DATE);
                if (xDate != null)
                    feed.Updated = FeedProcessor.ParseDateTime((string)xDate, feed.Updated);
            }
        }

        private static void SidewikiFeeds(Channel channel, string atomFeed)
        {
            XElement xChannel = XElement.Parse(atomFeed);
            //Book Search API uses dublin core for book information so fixing feeds

            foreach (Feed feed in channel.Feeds)
            {
                XElement xFeed = (from entry in xChannel.Elements(FeedProcessor.atomNS + FeedProcessor.XML_ENTRY)
                                  where feed.Link != null && feed.Link.HRef == (string)entry.Element(FeedProcessor.atomNS + FeedProcessor.XML_ID)
                                  select entry).FirstOrDefault();

                if (xFeed == null)
                    continue;

                XElement xLink = (from link in xFeed.Elements(FeedProcessor.atomNS + FeedProcessor.XML_LINK)
                                  where (string)link.Attribute(FeedProcessor.XML_REL) == "alternate"
                                        && (link.Attribute(FeedProcessor.XML_TYPE) == null
                                            || (string)link.Attribute(FeedProcessor.XML_TYPE) == "text/html")
                                  select link).FirstOrDefault();

                if (xLink != null)
                    feed.Link = new Link
                    {
                        LinkRel = LinkRel.Alternate,
                        Title = feed.Title,
                        HRef = (string)xLink.Attribute(FeedProcessor.XML_HREF),
                        Type = (string)xLink.Attribute(FeedProcessor.XML_TYPE)
                    };
            }
        }
    }
}
