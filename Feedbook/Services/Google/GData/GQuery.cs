using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Globalization;
using System.Web;

namespace Feedbook.Services.Google.GData
{
    internal class GQuery
    {     
        public string ServiceUrl { get; private set; }

        public string OrderBy { get; set; }

        public bool SortOrderAscending { get; set; }

        public string Author { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? PublishedMin { get; set; }

        public DateTime? PublishedMax { get; set; }

        public int? StartIndex { get; set; }

        public int? MaxResults { get; set; }

        public List<string> SearchWords { get; private set; }

        public GQuery(string serviceUrl)
        {
            this.ServiceUrl = serviceUrl;
            this.SearchWords = new List<string>();
        }

        public virtual string QueryParameters()
        {
            string paramInsertion = "?";
            StringBuilder queryBuilder = new StringBuilder();

            if (this.SearchWords.Count > 0)
            {
                string[] words = this.SearchWords.Where(w => !string.IsNullOrEmpty(w))
                                                 .Select(delegate(string w)
                                                            {
                                                                if (w.Contains(" "))
                                                                    return HttpUtility.UrlEncode("\"" + w + "\"");
                                                                return HttpUtility.UrlEncode(w);
                                                            }
                                                        ).ToArray();

                queryBuilder.Append(paramInsertion);
                queryBuilder.Append("q");
                queryBuilder.Append("=");
                queryBuilder.Append(string.Join(HttpUtility.UrlEncode("|"), words)); // | => OR
                paramInsertion = "&";
            }

            if (this.Author != null)
            {
                queryBuilder.Append(paramInsertion);
                queryBuilder.Append("author");
                queryBuilder.Append("=");
                queryBuilder.Append(HttpUtility.UrlEncode(this.Author));
                paramInsertion = "&";
            }

            if (this.StartDate != null)
            {
                queryBuilder.Append(paramInsertion);
                queryBuilder.Append("updated-min");
                queryBuilder.Append("=");
                queryBuilder.Append(HttpUtility.UrlEncode(LocalDateTimeInUTC(this.StartDate.Value)));
                paramInsertion = "&";
            }

            if (this.EndDate != null)
            {
                queryBuilder.Append(paramInsertion);
                queryBuilder.Append("updated-max");
                queryBuilder.Append("=");
                queryBuilder.Append(HttpUtility.UrlEncode(LocalDateTimeInUTC(this.EndDate.Value)));
                paramInsertion = "&";
            }

            if (this.PublishedMin != null)
            {
                queryBuilder.Append(paramInsertion);
                queryBuilder.Append("published-min");
                queryBuilder.Append("=");
                queryBuilder.Append(HttpUtility.UrlEncode(LocalDateTimeInUTC(this.PublishedMin.Value)));
                paramInsertion = "&";
            }

            if (this.PublishedMax != null)
            {
                queryBuilder.Append(paramInsertion);
                queryBuilder.Append("published-max");
                queryBuilder.Append("=");
                queryBuilder.Append(HttpUtility.UrlEncode(LocalDateTimeInUTC(this.PublishedMax.Value)));
                paramInsertion = "&";
            }

            if (this.StartIndex != null)
            {
                queryBuilder.Append(paramInsertion);
                queryBuilder.Append("start-index");
                queryBuilder.Append("=");
                queryBuilder.Append(this.StartIndex);
                paramInsertion = "&";
            }

            if (this.MaxResults != null)
            {
                queryBuilder.Append(paramInsertion);
                queryBuilder.Append("max-results");
                queryBuilder.Append("=");
                queryBuilder.Append(this.MaxResults);
                paramInsertion = "&";
            }

            return queryBuilder.ToString();
        }

        public override string ToString()
        {
            string queryParameters = this.QueryParameters();
            if (string.IsNullOrEmpty(queryParameters))
                return this.ServiceUrl;

            return this.ServiceUrl + queryParameters;
        }

        public static string LocalDateTimeInUTC(DateTime dateTime)
        {            
            TimeSpan timeZoneOffset = DateTimeOffset.Now.Offset;

            // Add "full-date T partial-time"
            string dateTimeStr = dateTime.ToUniversalTime().ToString("s", CultureInfo.InvariantCulture);

            // Add "time-offset"
            if (timeZoneOffset == TimeSpan.Zero)
                return dateTimeStr + "Z";

            TimeSpan duration = timeZoneOffset.Duration();
            if (timeZoneOffset < TimeSpan.Zero)
                return string.Format("{0}-{1:00}:{2:00}", dateTimeStr, duration.Hours, duration.Minutes);

            return string.Format("{0}+{1:00}:{2:00}", dateTimeStr, duration.Hours, duration.Minutes);
        }
    }
}
