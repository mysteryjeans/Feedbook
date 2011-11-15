using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Feedbook.Specifications.PortableContacts
{
    [XmlRoot(ElementName = "entry", Namespace = "http://portablecontacts.net/ns/1.0")]
    public class Contact
    {
        [XmlElement(ElementName="id")]
        public string Id { get; set; }

        [XmlElement(ElementName = "displayName")]
        public string DisplayName { get; set; }

        [XmlElement(ElementName = "profileUrl")]
        public string ProfileUrl { get; set; }

        [XmlElement(ElementName="urls")]
        public Url[] Urls { get; set; }

        [XmlElement(ElementName="photos")]
        public Photo[] Photos { get; set; }
    }
}
