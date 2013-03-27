using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using Feedbook.Helper;
using System.Web;

namespace Feedbook.Services.Bitly
{
    internal static class BitlyService
    {
        private const string Login = "feedbook";

        private const string ApiKey = "R_598b33a6869f9d510e2a8249bb49c199";

        private const string SHORTEN_URL = "http://api.bit.ly/v3/shorten";

        public static BitlyResponse ShortenUrl(string url)
        {
            string queryUrl = string.Format("{0}?login={1}&apiKey={2}&uri={3}&format=json", SHORTEN_URL, Login, ApiKey, HttpUtility.UrlEncode(url));
            using (Stream stream = Downloader.Download(queryUrl))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(BitlyResponse));
                return serializer.ReadObject(stream) as BitlyResponse;
            }
        }
    }
}
