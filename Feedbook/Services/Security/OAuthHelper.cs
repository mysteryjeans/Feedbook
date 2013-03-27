using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using Feedbook.Helper;
using System.IO;
using System.Collections.Specialized;
using CoreSystem.Util;
using System.Web;


namespace Feedbook.Services.Security
{
    internal static class OAuthHelper
    {
        public const string OAUTH_CONSUMER_KEY = "oauth_consumer_key";
        public const string OAUTH_NONCE = "oauth_nonce";
        public const string OAUTH_TIMESTAMP = "oauth_timestamp";
        public const string OAUTH_VERSION = "oauth_version";
        public const string OAUTH_TOKEN = "oauth_token";
        public const string OAUTH_SIGNATURE_METHOD = "oauth_signature_method";
        public const string OAUTH_SIGNATURE = "oauth_signature";
        public const string OAUTH_TOKEN_SECRET = "oauth_token_secret";
        public const string OAUTH_VERIFIER = "oauth_verifier";

        public static string OAUTHVERSION_1_0 = "1.0";

        private const string UNRESERVED_CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

        private static readonly Random random = new Random();

        public static void GetToken(OAuthSignatureBase oauthSignatureBase, string consumerSecret, string oauthTokenSecret, out string token, out string tokenSecret)
        {
            string url;
            string signature;
            string normalizedUrl;
            string normalizedParameters;

            Guard.CheckNull(oauthSignatureBase, "GetToken(oauthSignatureBase)");

            signature = oauthSignatureBase.GenerateSignature(consumerSecret, oauthTokenSecret, out normalizedUrl, out normalizedParameters);
            url = normalizedUrl + "?" + normalizedParameters + "&" + OAUTH_SIGNATURE + "=" + OAuthHelper.UrlEncode(signature);

            ParseToken(WebRequest(HttpMethod.GET, url, null), out token, out tokenSecret);
        }

        public static string OAuthGetWebRequest(string url, string consumerKey, string consumerSecret, string token, string tokenSecret)
        {
            return OAuthWebRequest(HttpMethod.GET, url, null, consumerKey, consumerSecret, token, tokenSecret);
        }

        public static string OAuthWebRequest(HttpMethod method, string url, PostData data, string consumerKey, string consumerSecret, string token, string tokenSecret)
        {
            string signature;
            string normalizedUrl;
            string normalizedParameters;
            OAuthSignatureBase signatureBase;

            Guard.CheckNullOrEmpty(url, "OAuthGetWebRequest(url)");

            if (string.IsNullOrEmpty(token))
                signatureBase = new OAuthSignatureBase(method, new Uri(url), consumerKey);
            else
                signatureBase = new OAuthSignatureBase(method, new Uri(url), consumerKey, token);

            signature = signatureBase.GenerateSignature(consumerSecret, tokenSecret, out normalizedUrl, out normalizedParameters);
            url = normalizedUrl + "?" + normalizedParameters + "&" + OAuthHelper.OAUTH_SIGNATURE + "=" + OAuthHelper.UrlEncode(signature);

            return WebRequest(method, url, data);
        }

        public static string WebRequest(HttpMethod method, string url, PostData data)
        {
            HttpWebRequest webRequest = (HttpWebRequest)System.Net.WebRequest.Create(url);
            webRequest.Method = method.ToString();
            webRequest.UserAgent = Constants.APPLICATION_NAME;
            webRequest.SetProxy();

            if (data != null)
                foreach (string header in data.Headers.Keys)
                    webRequest.Headers[header] = data.Headers[header];

            if (method == HttpMethod.POST || method == HttpMethod.PUT)
            {
                if (data != null)
                {
                    webRequest.ContentType = data.ContentType;

                    if (data.IsBinary)
                    {
                        webRequest.ContentLength = data.ContentLength;
                        int readBytes;
                        byte[] buffer = new byte[1024 * 10]; //10 KB                      
                        using (Stream requestStream = webRequest.GetRequestStream())
                        {
                            while ((readBytes = data.Binary.Read(buffer, 0, buffer.Length)) != 0)
                            {
                                requestStream.Write(buffer, 0, readBytes);
                            }
                        }
                    }
                    else
                    {
                        using (Stream requestStream = webRequest.GetRequestStream())
                        {
                            using (StreamWriter writer = new StreamWriter(requestStream))
                            {
                                writer.Write(data.Content);
                                writer.Close();
                            }
                        }
                    }
                }
                else webRequest.ContentLength = 0;
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

        public static void ParseToken(string response, out string token, out string tokenSecret)
        {
            if (response != null && response.Length > 0)
            {
                //Store the Token and Token Secret
                var queryString = OAuthHelper.ParseQueryString(response);
                if (queryString[OAuthHelper.OAUTH_TOKEN] != null)
                    token = queryString[OAUTH_TOKEN];
                else
                    throw new InvalidOperationException(string.Format("Recieved invalid response '{0}' from service, expecting token ...", response));

                if (queryString[OAUTH_TOKEN_SECRET] != null)
                    tokenSecret = queryString[OAUTH_TOKEN_SECRET];
                else
                    throw new InvalidOperationException(string.Format("Recieved invalid response '{0}' from service, expecting token secret ...", response));
            }
            else
                throw new InvalidOperationException(string.Format("Recieved empty response '{0}' from service", response));
        }

        public static string UrlEncode(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            StringBuilder encodedUrl = new StringBuilder();
            foreach (char symbol in value)
            {
                if (UNRESERVED_CHARS.IndexOf(symbol) != -1)
                    encodedUrl.Append(symbol);
                else
                    encodedUrl.Append(string.Format("%{0:X2}", (int)symbol));

            }

            return encodedUrl.ToString();
        }

        /// <summary>
        /// Generate the timestamp for the signature        
        /// </summary>
        /// <returns></returns>
        public static string GenerateTimeStamp()
        {
            // Default implementation of UNIX time of the current UTC time          
            return Convert.ToInt64((DateTime.UtcNow - Constants.UnixReferenceDate).TotalSeconds).ToString();
        }

        /// <summary>
        /// Generate a nonce
        /// </summary>
        /// <returns></returns>
        public static string GenerateNonce()
        {
            return random.Next(123400, int.MaxValue).ToString("X8");
        }

        public static Dictionary<string, string> ParseQueryString(string queryString)
        {
            var pairCollection = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(queryString))
            {
                queryString = queryString.TrimStart('?');
                string[] pairs = queryString.Split('&');
                if (pairs != null)
                {
                    foreach (string pairString in pairs)
                    {
                        string[] pair = pairString.Split('=');

                        if (pair != null && pair.Length > 0)
                            pairCollection.Add(HttpUtility.UrlDecode(pair[0]), (pair.Length > 1 ? HttpUtility.UrlDecode(pair[1]) : null));
                    }
                }
            }

            return pairCollection;
        }

        public static byte[] Scramble(byte[] scrambleBytes)
        {
            if (scrambleBytes == null)
                throw new ArgumentNullException("scrambleBytes");

            byte[] bytes = new byte[scrambleBytes.Length];

            for (int i = 0; i < scrambleBytes.Length; i += 2)
            {
                if (i + 1 < scrambleBytes.Length)
                {
                    bytes[i] = scrambleBytes[i + 1];
                    bytes[i + 1] = scrambleBytes[i];
                }
                else
                    bytes[i] = scrambleBytes[i];
            }

            return bytes;
        }
    }
}
