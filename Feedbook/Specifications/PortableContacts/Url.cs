using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Feedbook.Specifications.PortableContacts
{
    [XmlRoot(ElementName = "urls", Namespace = "http://portablecontacts.net/ns/1.0")]
    public class Url
    {
        [XmlElement(ElementName = "value")]
        public string Ref { get; set; }

        [XmlElement(ElementName = "type")]
        public string Type { get; set; }
    }
}
