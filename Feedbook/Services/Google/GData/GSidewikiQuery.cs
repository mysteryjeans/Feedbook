using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;


namespace Feedbook.Services.Google.GData
{
    internal class GSidewikiQuery : GQuery
    {
        internal static class QueryFor
        {
            public const string AUTHOR = "author";

            public const string WEBPAGE = "webpage";

            public const string DOMAINPATH = "domainpath";
        }

        public GSidewikiQuery(string queryFor, string queryForValue)
            : base(GetServiceUrl(queryFor, queryForValue))
        { }

        public GSidewikiQuery()
            : this(null, null)
        { }

        private static string GetServiceUrl(string queryFor, string queryForValue)
        {
            if (string.IsNullOrEmpty(queryFor))
                return GService.SIDEWIKI_SERVICE + "/all/full";

            return GService.SIDEWIKI_SERVICE + "/" + queryFor + "/" + HttpUtility.UrlEncode(queryForValue.TrimEnd('/')) + "/full";
        }
    }
}
