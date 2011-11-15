using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using CoreSystem.Util;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using Feedbook.Helper;
using Feedbook.Services.Security;
using Feedbook.Specifications.Twitter;
using System.Threading;
using System.Web;


namespace Feedbook.Services.Twitter
{
    internal static class TwitterService
    {
        #region Twitter Service Endpoints

        private const string TWEET_URL = "http://twitter.com/statuses/update.xml";

        private const string FRIENDS_URL = "http://api.twitter.com/1/statuses/friends.json";

        private const string SHOW_URL = "http://api.twitter.com/1/users/show.json";

        private const string XAUTH_ACCESS_TOKEN_URL = "https://api.twitter.com/oauth/access_token";

        private const string RATE_LIMIT_STATUS_URL = "http://api.twitter.com/1/account/rate_limit_status.json";

        private const string HOME_TIMELINE_URL = "http://api.twitter.com/1/statuses/home_timeline.atom";

        private const string USER_TIMELINE_URL = "http://api.twitter.com/1/statuses/user_timeline.atom";

        private const string FRIENDS_TIMELINE_URL = "http://api.twitter.com/1/statuses/friends_timeline.atom";

        private const string DISTORY_STATUS_URL = "http://api.twitter.com/1/statuses/destroy";

        private const string RETWEET_URL = "http://api.twitter.com/1/statuses/retweet";

        private const string FOLLOW_URL = "http://api.twitter.com/1/friendships/create";

        private const string UNFOLLOW_URL = "http://api.twitter.com/1/friendships/destroy";

        private const string DIRECT_MESSAGES_URL = "http://api.twitter.com/1/direct_messages.atom";

        private const string MENTIONS_URL = "http://api.twitter.com/1/statuses/mentions.atom";

        #endregion

        #region Twitter Consumer Key

        private static readonly byte[] ConsumerKeyBytes = new byte[] { 0x51, 0x36, 0x64, 0x74, 0x4F, 0x41, 0x74, 0x6A, 0x47, 0x68, 0x30, 0x48, 0x61, 0x70, 0x4D, 0x7A, 0x5A, 0x77, 0x73, 0x38, 0x41, 0x47 };

        private static readonly byte[] ConsumerSecretBytes = new byte[] { 0x66, 0x75, 0x70, 0x55, 0x47, 0x67, 0x35, 0x50, 0x4A, 0x79, 0x4B, 0x61, 0x34, 0x4A, 0x50, 0x31, 0x72, 0x45, 0x6C, 0x76, 0x4D, 0x52, 0x53, 0x65, 0x5A, 0x65, 0x6F, 0x6B, 0x32, 0x53, 0x37, 0x41, 0x41, 0x59, 0x70, 0x64, 0x35, 0x30, 0x6B, 0x70 };

        #endregion

        public static void GetAccessToken(string username, string password, out string token, out string tokenSecret)
        {
            Guard.CheckNullOrEmpty(username, "GetAccessToken(username)");
            Guard.CheckNullOrEmpty(password, "GetAccessToken(password)");

            string url = string.Format("{0}?x_auth_username={1}&x_auth_password={2}&x_auth_mode=client_auth", XAUTH_ACCESS_TOKEN_URL, HttpUtility.UrlEncode(username), HttpUtility.UrlEncode(password));
            string response = OAuthGetWebRequest(url, null, null);

            OAuthHelper.ParseToken(response, out token, out tokenSecret);
        }

        public static TwitterRateLimit GetRateLimit()
        {
            string jsonResponse = WebRequest(HttpMethod.GET, RATE_LIMIT_STATUS_URL, null);

            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonResponse)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TwitterRateLimit));
                return serializer.ReadObject(stream) as TwitterRateLimit;
            }
        }

        public static TwitterRateLimit GetRateLimit(string token, string tokenSecret)
        {
            string jsonResponse = OAuthGetWebRequest(RATE_LIMIT_STATUS_URL, token, tokenSecret);

            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonResponse)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TwitterRateLimit));
                return serializer.ReadObject(stream) as TwitterRateLimit;
            }
        }

        public static TwitterUser Show(string username)
        {
            HttpWebRequest request = (HttpWebRequest)System.Net.WebRequest.Create(string.Format("{0}?screen_name={1}", SHOW_URL, username));

            request.Method = "GET";
            //request.UserAgent = Constants.APPLICATION_NAME;

            using (WebResponse response = request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TwitterUser));
                    var users = serializer.ReadObject(responseStream);
                    return users as TwitterUser;
                }
            }
        }

        public static TwitterUser Show(string username, string token, string tokenSecret)
        {
            Guard.CheckNullOrEmpty(username, "Show(username)");
            string url = string.Format("{0}?screen_name={1}", SHOW_URL, username.ToLower());
            string respsone = OAuthGetWebRequest(url, token, tokenSecret);

            using (Stream responseStream = new MemoryStream(Encoding.UTF8.GetBytes(respsone)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TwitterUser));
                var users = serializer.ReadObject(responseStream);
                return users as TwitterUser;
            }
        }

        public static void Follow(string userId, string token, string tokenSecret)
        {
            Guard.CheckNullOrEmpty(userId, "Follow(userId)");
            string url = string.Format("{0}/{1}.xml", FOLLOW_URL, userId.ToLower());
            OAuthWebRequest(HttpMethod.POST, url, null, token, tokenSecret);
        }

        public static void UnFollow(string userId, string token, string tokenSecret)
        {
            Guard.CheckNullOrEmpty(userId, "UnFollow(userId)");
            string url = string.Format("{0}/{1}.xml", UNFOLLOW_URL, userId.ToLower());
            OAuthWebRequest(HttpMethod.POST, url, null, token, tokenSecret);
        }

        public static string Update(string token, string tokenSecret, string status)
        {
            string url = TWEET_URL + "?status=" + HttpUtility.UrlEncode(status);
            return OAuthWebRequest(HttpMethod.POST, url, null, token, tokenSecret);
        }

        public static void Distory(string statusId, string token, string tokenSecret)
        {
            Guard.CheckNullOrEmpty(statusId, "Distory(statusId)");

            string url = string.Format("{0}/{1}.xml", DISTORY_STATUS_URL, statusId);
            OAuthWebRequest(HttpMethod.DELETE, url, null, token, tokenSecret);
        }

        public static void Retweet(string statusId, string token, string tokenSecret)
        {
            Guard.CheckNullOrEmpty(statusId, "Retweet(statusId)");

            string url = string.Format("{0}/{1}.xml", RETWEET_URL, statusId);
            OAuthWebRequest(HttpMethod.POST, url, null, token, tokenSecret);
        }

        public static string GetTimeline(Timelines timeline, string token, string tokenSecret)
        {
            return OAuthGetWebRequest(GetTimelineUrl(timeline), token, tokenSecret);
        }

        public static string GetTimeline(string timelineUrl, string sinceStatusId, string token, string tokenSecret)
        {
            timelineUrl = (new Uri(timelineUrl)).AbsoluteUri;
            if (!string.IsNullOrEmpty(sinceStatusId))
                timelineUrl += "?since_id=" + sinceStatusId;

            return OAuthGetWebRequest(timelineUrl, token, tokenSecret);
        }

        public static TwitterUserCollection Friends(string token, string tokenSecret)
        {
            string jsonResponse = OAuthGetWebRequest(FRIENDS_URL, token, tokenSecret);

            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonResponse)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TwitterUserCollection));
                var users = serializer.ReadObject(stream);
                return users as TwitterUserCollection;
            }
        }

        public static string Search(string searchUrl, string sinceStatusId)
        {
            if (!string.IsNullOrEmpty(sinceStatusId))
                searchUrl = string.Format("{0}&since_id={1}", searchUrl, sinceStatusId);

            return WebRequest(HttpMethod.GET, searchUrl, null);
        }

        public static string GetTimelineUrl(Timelines timeline)
        {
            switch (timeline)
            {
                case Timelines.Friends:
                    return FRIENDS_TIMELINE_URL;
                case Timelines.User:
                    return USER_TIMELINE_URL;
                case Timelines.Home:
                    return HOME_TIMELINE_URL;
                case Timelines.DirectMessages:
                    return DIRECT_MESSAGES_URL;
                case Timelines.Replies:
                    return MENTIONS_URL;
                default:
                    throw new NotSupportedException(string.Format("Twitter doesn't supported time line: '{0}'", timeline));
            }
        }

        private static string OAuthGetWebRequest(string url, string token, string tokenSecret)
        {
            return OAuthWebRequest(HttpMethod.GET, url, null, token, tokenSecret);
        }

        private static string OAuthWebRequest(HttpMethod method, string url, string postData, string token, string tokenSecret)
        {
            string signature;
            string normalizedUrl;
            string normalizedParameters;
            OAuthSignatureBase signatureBase;

            Guard.CheckNullOrEmpty(url, "OAuthGetWebRequest(url)");

            if (string.IsNullOrEmpty(token))
                signatureBase = new OAuthSignatureBase(method, new Uri(url), GetConsumerKey());
            else
                signatureBase = new OAuthSignatureBase(method, new Uri(url), GetConsumerKey(), token);

            signature = signatureBase.GenerateSignature(GetConsumerSecret(), tokenSecret, out normalizedUrl, out normalizedParameters);
            url = normalizedUrl + "?" + normalizedParameters + "&" + OAuthHelper.OAUTH_SIGNATURE + "=" + OAuthHelper.UrlEncode(signature);

            return WebRequest(method, url, null);
        }

        private static string WebRequest(HttpMethod method, string url, string postData)
        {
            HttpWebRequest webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = method.ToString();
            webRequest.UserAgent = Constants.APPLICATION_NAME;
            webRequest.SetProxy();

            if (method == HttpMethod.POST && !string.IsNullOrEmpty(postData))
            {
                webRequest.ContentType = "application/x-www-form-urlencoded";

                //POST the data.              
                using (Stream requestStream = webRequest.GetRequestStream())
                {
                    using (StreamWriter writer = new StreamWriter(requestStream))
                    {
                        writer.Write(postData);
                        writer.Close();
                    }
                }
            }

            using (WebResponse response = webRequest.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        private static string GetConsumerKey()
        {
            return Encoding.UTF8.GetString(OAuthHelper.Scramble(ConsumerKeyBytes), 0, ConsumerKeyBytes.Length);
        }

        private static string GetConsumerSecret()
        {
            return Encoding.UTF8.GetString(OAuthHelper.Scramble(ConsumerSecretBytes), 0, ConsumerSecretBytes.Length);
        }
    }
}
