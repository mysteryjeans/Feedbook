using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;

namespace Feedbook
{
    internal static class Constants
    {
        public const string APPLICATION_NAME = "Feedbook";

        public const string ICON_URL = "https://lh6.googleusercontent.com/_o-iIggNHfDU/TD8kde_JezI/AAAAAAAAAIY/VyOvUy8cWIw/Feedbook%20Logo-256x256.png";

        public const string BLOG_URL = "http://blog.feedbook.org";

        public const string WEBSITE_URL = "http://feedbook.org";

        public const string XML_TAG_REGEX = @"</?\w+\s+[^>]*>";

        public const string ANCHOR_LINK_TAG_REGEX = @"<(?<tag>a|link)[^>]*>";

        public const string IMAGE_TAG_REGEX = @"<(?<tag>img)[^>]*>";

        public const string HREF_ATTRIBUTE_REGEX = "href\\s*=\\s*(?:\"(?<url>[^\"]*)\"|(?<url>\\S+))";

        public const string SRC_ATTRIBUTE_REGEX = "src\\s*=\\s*(?:\"(?<src>[^\"]*)\"|(?<src>\\S+))";

        public const string TYPE_ATTRIBUTE_REGEX = "type\\s*=\\s*(?:\"(?<type>[^\"]*)\"|(?<type>\\S+))";

        public const string RESX_RSS_IMAGE_URI = "/Feedbook;component/Images/RSS.png";

        public const string RESX_TWITTER_IMAGE_URI = "/Feedbook;component/Images/Twitter.png";

        public const string RESX_GOOGLE_BUZZ_IMAGE_URI = "/Feedbook;component/Images/Buzz.png";

        public const string RESX_AUDIO_IMAGE_URI = "/Feedbook;component/Images/audio.png";

        public const string RESX_VIDEO_IMAGE_URI = "/Feedbook;component/Images/video.png";

        public const string RESX_ATTACHMENT_IMAGE_URI = "/Feedbook;component/Images/attachment.png";

        public static readonly DateTime UnixReferenceDate = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        internal static class Default
        {
            public const int ShowMilliseconds = 300;

            public const int HideMilliseconds = 400;

            public const int PopupDurationMilliseconds = 3000;

            public static readonly string[] PresetRssUrls = new string[] {  
                                                                            "http://feeds.feedburner.com/blogspot/UDgrP",
                                                                            "http://channel9.msdn.com/Feeds/RSS",
                                                                            "http://feeds.feedburner.com/TechCrunch"
                                                                          };
        }

        internal static class Resources
        {
            public const string IMAGE_AUDIO = @"/Feedbook;component/Images/audio.png";

            public const string IMAGE_VIDEO = @"/Feedbook;component/Images/video.png";

            public const string IMAGE_APPLICATION = @"/Feedbook;component/Images/application.png";

            public const string IMAGE_ENCLOSURE = @"/Feedbook;component/Images/enclosure.png";

            public const string IMAGE_ATTACHMENT = @"/Feedbook;component/Images/attachment.png";

            public const string IMAGE_DOWNLOAD = @"/Feedbook;component/Images/download.png";

            public const string IMAGE_RSS = @"/Feedbook;component/Images/rss_2.png";

            public const string IMAGE_GBUZZ = @"/Feedbook;component/Images/buzz.png";

            public const string IMAGE_TWITTER = @"/Feedbook;component/Images/twitter.png";

            public const string IMAGE_INFO = @"/Feedbook;component/Images/info.png";
        }

        internal static class Caption
        {
            public const string APP_NAME = "Feedbook";

            public const string INFO = "Information";

            public const string ERROR = "Error";

            public const string EXCEPTION = "Exception";

            public const string WARNING = "Warning";
        }

        internal static class SysConfig
        {
            public const int MaxTwitterFeed = 20;

            public const int MaxBuzzFeed = 20;

            public const int MaxWebFeed = 30;

            public const int MaxLogs = 10;

            public const string FeedbookEmail = "info@feedbook.org";

            public static readonly TimeSpan SynchronizeInterval = TimeSpan.FromMinutes(3);

            public static readonly string DownloadFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Constants.APPLICATION_NAME);
        }

        public static string BytesToHex(byte[] bytes)
        {
            string hexStr = null;
            foreach (byte b in bytes)
                hexStr += b.ToString("x2");

            return hexStr;
        }        
    }
}
