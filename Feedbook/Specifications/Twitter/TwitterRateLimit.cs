using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Feedbook.Specifications.Twitter
{
    [DataContract]
    internal class TwitterRateLimit
    {
        [DataMember(Name = "remaining_hits")]
        public int RemainingHits { get; set; }

        [DataMember(Name = "hourly_limit")]
        public int HourlyLimit { get; set; }

        //[DataMember(Name = "reset_time")]
        public DateTime ResetTime
        {
            get { return Constants.UnixReferenceDate.AddSeconds(this.ResetTimeInSeconds); }
        }

        [DataMember(Name = "reset_time_in_seconds")]
        public int ResetTimeInSeconds { get; set; }
    }
}
