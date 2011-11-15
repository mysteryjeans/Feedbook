using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Feedbook.Model
{
    [DataContract]
    internal enum AccountType
    {
        [EnumMember]
        Twitter,

        [EnumMember]
        Google
    }
}
