using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Feedbook.Specifications.PortableContacts
{

    [XmlRoot(ElementName = "response", Namespace = "http://portablecontacts.net/ns/1.0")]
    public class Response
    {
        [XmlElement(ElementName="entry")]
        public Contact[] Contacts { get; set; }
    }
}
