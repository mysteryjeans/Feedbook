using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Feedbook.Helper;
using System.Collections.ObjectModel;
using CoreSystem.RefTypeExtension;
using CoreSystem.Util;
using CoreSystem.ValueTypeExtension;
using System.Security.Cryptography;
using System.Collections.Specialized;
using System.Net;


namespace Feedbook.Services.Security
{
    internal class OAuthSignatureBase
    {
        public HttpMethod Method { get; private set; }

        public Uri Url { get; private set; }

        public string Nonce { get; private set; }

        public string TimeStamp { get; private set; }

        public string OAuthVersion { get; private set; }

        public SignatureMethod SignatureMethod { get; private set; }

        public ReadOnlyCollection<QueryParameter> QueryParameters { get; private set; }

        public OAuthSignatureBase(HttpMethod method, Uri url, IEnumerable<QueryParameter> queryParameters)
        {
            this.Method = method;
            this.Url = url;
            this.Nonce = OAuthHelper.GenerateNonce();
            this.TimeStamp = OAuthHelper.GenerateTimeStamp();
            this.OAuthVersion = OAuthHelper.OAUTHVERSION_1_0;

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.AddRange(GetQueryParameters(url));

            if (queryParameters != null)
                parameters.AddRange(queryParameters);

            this.QueryParameters = parameters.ToReadOnly();
        }

        public OAuthSignatureBase(HttpMethod method, Uri url) :
            this(method, url, (IEnumerable<QueryParameter>)null)
        { }

        public OAuthSignatureBase(HttpMethod method, Uri url, string consumerKey) :
            this(method, url, new QueryParameter[] { new QueryParameter(OAuthHelper.OAUTH_CONSUMER_KEY, consumerKey) })
        {
            Guard.CheckNullOrEmpty(consumerKey, "OAuthSignatureBase(consumerKey)");
        }

        public OAuthSignatureBase(HttpMethod method, Uri url, string consumerKey, string token) :
            this(method, url, new QueryParameter[] { new QueryParameter(OAuthHelper.OAUTH_CONSUMER_KEY, consumerKey)
                                                    ,new QueryParameter(OAuthHelper.OAUTH_TOKEN, token)})
        {
            Guard.CheckNullOrEmpty(consumerKey, "OAuthSignatureBase(consumerKey)");
            Guard.CheckNullOrEmpty(consumerKey, "OAuthSignatureBase(token)");
        }

        private IEnumerable<QueryParameter> GetQueryParameters(Uri url)
        {
            if (!string.IsNullOrEmpty(url.Query))
            {
                //Decode the parameters and re-encode using the oAuth UrlEncode method.
                var parameters = OAuthHelper.ParseQueryString(url.Query);

                foreach (string key in parameters.Keys)
                    yield return new QueryParameter(key, parameters[key]);
            }
        }

        public string GenerateSignature(string consumerSecret, string tokenSecret, out string normalizedUrl, out string normalizedParameters)
        {
            Guard.CheckNullOrEmpty(consumerSecret, "GenerateSignature(ConsumerSecret)");
            string signatureBase = this.GetSignatureBase(out normalizedUrl, out normalizedParameters);

            switch (this.SignatureMethod)
            {
                case SignatureMethod.HMAC_SHA1:
                    var hmacsha1 = new HMACSHA1();
                    hmacsha1.Key = Encoding.UTF8.GetBytes(string.Format("{0}&{1}", OAuthHelper.UrlEncode(consumerSecret), OAuthHelper.UrlEncode(tokenSecret)));
                    var hashBytes = hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(signatureBase));
                    return Convert.ToBase64String(hashBytes);
                default:
                    throw new NotSupportedException(string.Format("Signature Method not supported: '{0}'", this.SignatureMethod));
            }
        }

        public override string ToString()
        {
            string normalizedUrl;
            string normalizedParameters;
            return this.GetSignatureBase(out normalizedUrl, out normalizedParameters);
        }

        private string GetSignatureBase(out string normalizedUrl, out string normalizedParameters)
        {
            List<QueryParameter> queryParameters = new List<QueryParameter>();
            queryParameters.AddRange(this.QueryParameters);

            queryParameters.Add(new QueryParameter(OAuthHelper.OAUTH_NONCE, this.Nonce));
            queryParameters.Add(new QueryParameter(OAuthHelper.OAUTH_TIMESTAMP, this.TimeStamp));
            queryParameters.Add(new QueryParameter(OAuthHelper.OAUTH_VERSION, this.OAuthVersion));
            queryParameters.Add(new QueryParameter(OAuthHelper.OAUTH_SIGNATURE_METHOD, this.SignatureMethod.ToDescription()));

            queryParameters.Sort();

            normalizedUrl = string.Format("{0}://{1}", this.Url.Scheme, this.Url.Host);

            if (!((this.Url.Scheme == "http" && this.Url.Port == 80) || (this.Url.Scheme == "https" && this.Url.Port == 443)))
                normalizedUrl += ":" + this.Url.Port;

            normalizedUrl += this.Url.AbsolutePath;

            normalizedParameters = NormalizeRequestParameters(queryParameters);

            StringBuilder signatureBase = new StringBuilder();
            signatureBase.AppendFormat("{0}&", this.Method.ToString());
            signatureBase.AppendFormat("{0}&", OAuthHelper.UrlEncode(normalizedUrl));
            signatureBase.AppendFormat("{0}", OAuthHelper.UrlEncode(normalizedParameters));

            return signatureBase.ToString();
        }

        private static string NormalizeRequestParameters(IList<QueryParameter> parameters)
        {
            QueryParameter parameter = null;
            StringBuilder parameterStrBuilder = new StringBuilder();

            for (int i = 0; i < parameters.Count; i++)
            {
                parameter = parameters[i];
                if (!string.IsNullOrEmpty(parameter.Name))
                    parameterStrBuilder.AppendFormat("{0}={1}", OAuthHelper.UrlEncode(parameter.Name), OAuthHelper.UrlEncode(parameter.Value));
                else
                    parameterStrBuilder.AppendFormat("{0}", OAuthHelper.UrlEncode(parameter.Value));

                if (i < parameters.Count - 1)
                    parameterStrBuilder.Append("&");
            }

            return parameterStrBuilder.ToString();
        }
    }
}
