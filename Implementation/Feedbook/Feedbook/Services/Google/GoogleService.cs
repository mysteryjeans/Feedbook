using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Feedbook.Services.Security;
using CoreSystem.Util;
using System.Net;
using Feedbook.Helper;
using System.Web;


namespace Feedbook.Services.Google
{
    internal abstract class GoogleService
    {
        #region Google Service EndPoints

        protected const string REQUEST_TOKEN_URL = "https://www.google.com/accounts/OAuthGetRequestToken";

        protected const string AUTHORIZATION_URL = "https://www.google.com/accounts/OAuthAuthorizeToken";

        protected const string ACCESS_TOKEN_URL = "https://www.google.com/accounts/OAuthGetAccessToken";

        #endregion

        #region Google Query Parameters

        protected const string SCOPE = "scope";

        protected const string XOAUTH_DISPLAYNAME = "xoauth_displayname";

        #endregion

        #region Feedbook Consumer Keys

        //protected static readonly byte[] CONSUMER_KEY_BYTES = new byte[] { 0x79, 0x6D, 0x65, 0x66, 0x64, 0x65, 0x6F, 0x62, 0x6B, 0x6F, 0x77, 0x2E, 
        //                                                                0x72, 0x6F, 0x70, 0x64, 0x65, 0x72, 0x73, 0x73, 0x63, 0x2E, 0x6D, 0x6F};


        //protected static readonly byte[] CONSUMER_SECRET_BYTES = new byte[] {0x47, 0x4C, 0x6E, 0x43, 0x33, 0x48, 0x73, 0x64, 0x51, 0x5A, 0x7A, 0x41, 
        //                                                                   0x57, 0x46, 0x56, 0x38, 0x41, 0x31, 0x4F, 0x73, 0x6F, 0x6F, 0x37, 0x6A};

        //protected static readonly byte[] CONSUMER_KEY_BYTES = new byte[] {0x79, 0x6D, 0x65, 0x66, 0x64, 0x65, 0x6F, 0x62, 0x6B, 0x6F, 0x62, 
        //                                                                   0x2E, 0x6F, 0x6C, 0x73, 0x67, 0x6F, 0x70, 0x2E, 0x74, 0x6F, 0x63, 0x6D};


        //protected static readonly byte[] CONSUMER_SECRET_BYTES = new byte[] {0x6F, 0x50, 0x30, 0x37, 0x32, 0x6F, 0x50, 0x37, 0x69, 0x38, 0x36, 0x79, 0x31,
        //                                                                     0x54, 0x33, 0x45, 0x48, 0x48, 0x76, 0x36, 0x36, 0x74, 0x63, 0x7A};

        // feedbook.org
        protected static readonly byte[] CONSUMER_KEY_BYTES = new byte[] { 0x65, 0x66, 0x64, 0x65, 0x6F, 0x62, 0x6B, 0x6F, 0x6F, 0x2E, 0x67, 0x72 };

        protected static readonly byte[] CONSUMER_SECRET_BYTES = new byte[] { 0x59, 0x4F, 0x4E, 0x4F, 0x32, 0x4C, 0x6A, 0x6A, 0x6C, 0x53, 0x61, 0x69, 
                                                                              0x70, 0x4E, 0x4A, 0x70, 0x57, 0x5A, 0x41, 0x47, 0x4C, 0x4E, 0x45, 0x31 };

        #endregion

        public abstract string ScopeUrl { get; }

        public virtual string AuthorizationUrl { get { return AUTHORIZATION_URL; } }

        public void GetRequestToken(out string token, out string tokenSecret)
        {
            string url = string.Format("{0}?scope={1}&xoauth_displayname={2}&oauth_callback={3}", REQUEST_TOKEN_URL, HttpUtility.UrlEncode(this.ScopeUrl), HttpUtility.UrlEncode(Constants.APPLICATION_NAME), HttpUtility.UrlEncode(Constants.WEBSITE_URL));

            OAuthSignatureBase signatureBase = new OAuthSignatureBase(HttpMethod.GET, new Uri(url), GetConsumerKey());
            OAuthHelper.GetToken(signatureBase, GetConsumerSecret(), null, out token, out tokenSecret);
        }

        public string GetAuthorizationUrl(string token)
        {
            Guard.CheckNullOrEmpty(token, "GetAuthorizationUrl(token)");
            return string.Format("{0}?oauth_token={1}&domain={2}&scope={3}&iconUrl={4}", this.AuthorizationUrl, HttpUtility.UrlEncode(token), HttpUtility.UrlEncode(GetConsumerKey()), HttpUtility.UrlEncode(this.ScopeUrl), HttpUtility.UrlEncode(Constants.ICON_URL));
        }

        public void GetAccessToken(string requestToken, string requestTokenSecret, string requestTokenVerifier, out string accessToken, out string accessTokenSecret)
        {
            Guard.CheckNullOrEmpty(requestToken, "GetAccessToken(requestToken)");
            Guard.CheckNullOrEmpty(requestTokenSecret, "GetAccessToken(requestTokenSecret)");
            Guard.CheckNullOrEmpty(requestTokenSecret, "GetAccessToken(requestTokenVerifier)");

            string url;
            OAuthSignatureBase signatureBase;

            url = ACCESS_TOKEN_URL + "?oauth_verifier=" + HttpUtility.UrlEncode(requestTokenVerifier);

            signatureBase = new OAuthSignatureBase(HttpMethod.GET, new Uri(url), GetConsumerKey(), requestToken);
            OAuthHelper.GetToken(signatureBase, GetConsumerSecret(), requestTokenSecret, out accessToken, out accessTokenSecret);
        }

        public static string OAuthGetWebRequest(string url, string token, string tokenSecret)
        {
            return OAuthHelper.OAuthGetWebRequest(url, GetConsumerKey(), GetConsumerSecret(), token, tokenSecret);
        }

        public static string OAuthWebRequest(HttpMethod method, string url, PostData data, string token, string tokenSecret)
        {
            return OAuthHelper.OAuthWebRequest(method, url, data, GetConsumerKey(), GetConsumerSecret(), token, tokenSecret);
        }

        private static string GetConsumerKey()
        {
            return Encoding.UTF8.GetString(OAuthHelper.Scramble(CONSUMER_KEY_BYTES), 0, CONSUMER_KEY_BYTES.Length);
        }

        private static string GetConsumerSecret()
        {
            return Encoding.UTF8.GetString(OAuthHelper.Scramble(CONSUMER_SECRET_BYTES), 0, CONSUMER_SECRET_BYTES.Length);
        }
    }
}
