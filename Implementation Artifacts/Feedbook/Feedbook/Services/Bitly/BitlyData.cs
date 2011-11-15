using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Feedbook.Services.Bitly
{
    [DataContract(Name = "data", Namespace = "")]
    internal class BitlyData
    {
        [DataMember(Name = "url")]
        public string ShortUrl { get; set; }

        [DataMember(Name = "hash")]
        public string Hash { get; set; }

        [DataMember(Name = "long_url")]
        public string OriginalUrl { get; set; }

        [DataMember(Name = "new_hash")]
        public string NewHash { get; set; }
    }
}
