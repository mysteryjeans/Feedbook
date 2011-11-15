using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Feedbook.Services.Security;
using System.Net;
using Feedbook.Helper;
using System.Collections.Specialized;
using CoreSystem.Util;
using PC = Feedbook.Specifications.PortableContacts;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using Feedbook.Specifications.PortableContacts;
using System.Xml.Serialization;
using Feedbook.Model;
using Feedbook.Services.Google.Picasa;


namespace Feedbook.Services.Google.Buzz
{
    internal class GBuzzService : GoogleService
    {
        #region Google Service EndPoints

        private const string BUZZ_AUTHORIZATION_URL = "https://www.google.com/buzz/api/auth/OAuthAuthorizeToken";

        private const string BUZZ_SCOPE_URL = "https://www.googleapis.com/auth/buzz";

        private const string ACTIVITY_FEED_URL = "https://www.googleapis.com/buzz/v1/activities";

        private const string CONTACTS_URL = "https://www.googleapis.com/buzz/v1/people";

        private const string FOLLOW_URL = "https://www.googleapis.com/buzz/v1/people/@me/@groups/@following";

        #endregion

        private const string ME = "@me";

        public override string ScopeUrl { get { return BUZZ_SCOPE_URL + " " + GPicasaService.PICASA_SCOPE_URL; } }

        public override string AuthorizationUrl { get { return BUZZ_AUTHORIZATION_URL; } }

        public static string GetActivityFeed(ActivityType activityType, string token, string tokenSecret)
        {
            return GetActivityFeed(GetActivityFeedUrl(activityType), token, tokenSecret);
        }

        public static string GetActivityFeed(string activityFeedUrl, string token, string tokenSecret)
        {
            return OAuthGetWebRequest(activityFeedUrl, token, tokenSecret);
        }

        public static string GetActivityFeedUrl(ActivityType activityType)
        {
            switch (activityType)
            {
                case ActivityType.Self:
                case ActivityType.Public:
                case ActivityType.Consumption:
                    return string.Format("{0}/{1}/@{2}", ACTIVITY_FEED_URL, ME, activityType.ToString().ToLower());
                default:
                    throw new NotSupportedException(string.Format("Google Buzz activity type: '{0}' not supported", activityType));
            }
        }

        public static PC.Contact[] GetContacts(ContactType contactType, string userId, string token, string tokenSecret)
        {
            string url = string.Format("{0}/{1}/", CONTACTS_URL, userId);
            switch (contactType)
            {
                case ContactType.All:
                    return ParsePortableContacts(OAuthGetWebRequest(url + "@all", token, tokenSecret));
                case ContactType.Self:
                    return new Contact[] { ParsePortableContact(OAuthGetWebRequest(url + "@self", token, tokenSecret)) };
                case ContactType.Following:
                    return ParsePortableContacts(OAuthGetWebRequest(url + "@groups/@following", token, tokenSecret));
                case ContactType.Followers:
                    return ParsePortableContacts(OAuthGetWebRequest(url + "@groups/@followers", token, tokenSecret));
                case ContactType.Blocked:
                    return ParsePortableContacts(OAuthGetWebRequest(url + "@groups/@blocked", token, tokenSecret));
                default:
                    throw new NotSupportedException(string.Format("Google buzz doesn't support contact type '{0}'", contactType));
            }
        }

        public static PC.Contact GetUserProfile(string token, string tokenSecret)
        {
            var contacts = GetContacts(ContactType.Self, ME, token, tokenSecret);

            if (contacts == null || contacts.Length == 0)
                throw new InvalidOperationException("Recieved empty user profile from google buzz service");

            if (contacts.Length != 1)
                throw new InvalidOperationException("Recieved more then one user profile from google buzz service");

            return contacts[0];
        }

        public static PC.Contact GetUserProfile(string userId, string token, string tokenSecret)
        {
            var contacts = GetContacts(ContactType.Self, userId, token, tokenSecret);

            if (contacts == null || contacts.Length == 0)
                throw new InvalidOperationException("Recieved empty user profile from google buzz service");

            if (contacts.Length != 1)
                throw new InvalidOperationException("Recieved more then one user profile from google buzz service");

            return contacts[0];
        }

        public static PC.Contact[] GetFriends(string token, string tokenSecret)
        {
            return GetContacts(ContactType.Following, ME, token, tokenSecret);
        }          

        public static void Follow(string personId, string token, string tokenSecret)
        {
            Guard.CheckNullOrEmpty(personId, "Follow(personId)");
            string url = string.Format("{0}/{1}",FOLLOW_URL,personId);
            OAuthWebRequest(HttpMethod.PUT, url, null, token, tokenSecret);
        }

        public static void UnFollow(string personId, string token, string tokenSecret)
        {
            Guard.CheckNullOrEmpty(personId, "UnFollow(personId)");
            string url = string.Format("{0}/{1}", FOLLOW_URL, personId);
            OAuthWebRequest(HttpMethod.DELETE, url, null, token, tokenSecret);
        }

        public static string CreateActivity(string atomEntry, string token, string tokenSecret)
        {
            Guard.CheckNullOrEmpty(atomEntry, "CreateActivity(atomEntry)");

            string url = ACTIVITY_FEED_URL + "/@me/@self";
            return OAuthWebRequest(HttpMethod.POST, url, new PostData(atomEntry, "application/atom+xml"), token, tokenSecret);
        }

        public static string CreateComments(string atomEntry, string userId, string activityId, string token, string tokenSecret)
        {
            Guard.CheckNullOrEmpty(atomEntry, "CreateComments(atomEntry)");
            Guard.CheckNullOrEmpty(userId, "CreateComments(userId)");
            Guard.CheckNullOrEmpty(activityId, "CreateComments(activityId)");

            string url = string.Format("{0}/{1}/@self/{2}/@comments", ACTIVITY_FEED_URL, userId, activityId);
            return OAuthWebRequest(HttpMethod.POST, url, new PostData(atomEntry, "application/atom+xml"), token, tokenSecret);
        }

        public static string LikeAcivity(string activityId, string token, string tokenSecret)
        {
            Guard.CheckNullOrEmpty(activityId, "LikeAcivity(activityId)");
            string url = string.Format("{0}/@me/@liked/{1}", ACTIVITY_FEED_URL, activityId);
            return OAuthWebRequest(HttpMethod.PUT, url, null, token, tokenSecret);
        }

        public static string UnLikeAcivity(string activityId, string token, string tokenSecret)
        {
            Guard.CheckNullOrEmpty(activityId, "UnLikeAcivity(activityId)");
            string url = string.Format("{0}/@me/@liked/{1}", ACTIVITY_FEED_URL, activityId);
            return OAuthWebRequest(HttpMethod.DELETE, url, null, token, tokenSecret);
        }

        public static void DeleteAcivity(string activityId, string token, string tokenSecret)
        {
            Guard.CheckNullOrEmpty(activityId, "DeleteActivity(activityId)");
            string url = string.Format("{0}/@me/@self/{1}", ACTIVITY_FEED_URL, activityId);
            OAuthWebRequest(HttpMethod.DELETE, url, null, token, tokenSecret);
        }

        public static PC.Contact[] ParsePortableContacts(string xmlResponse)
        {
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(xmlResponse)))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(PC.Response));
                var response = (Response)serializer.Deserialize(stream);
                return (response != null) ? response.Contacts : null;
            }
        }
      
        private static PC.Contact ParsePortableContact(string xmlResponse)
        {
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(xmlResponse)))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(PC.Contact));
                return (Contact)serializer.Deserialize(stream);
            }
        }
    }
}
