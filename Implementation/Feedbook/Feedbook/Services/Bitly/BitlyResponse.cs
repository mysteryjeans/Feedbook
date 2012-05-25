using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Feedbook.Services.Bitly
{
    [DataContract(Name="response", Namespace="")]
    internal class BitlyResponse
    {   
        [DataMember(Name="status_code")]
        public int StatusCode { get; set;}

        [DataMember(Name="status_txt")]
        public string StatusText { get; set;}

        [DataMember(Name="data")]
        public BitlyData Data { get ; set;}
    }
}
