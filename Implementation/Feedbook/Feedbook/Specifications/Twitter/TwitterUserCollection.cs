using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;

namespace Feedbook.Specifications.Twitter
{
    [CollectionDataContract(Name = "users", Namespace = "", ItemName = "user")]
    internal class TwitterUserCollection : List<TwitterUser>
    {
    }
}
