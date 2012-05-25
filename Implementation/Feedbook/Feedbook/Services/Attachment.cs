using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Feedbook.Model;

namespace Feedbook.Services
{
    internal class Attachment
    {
        public string Title { get; set;}

        public List<Link> Links { get; set; }

        public SocialType SocialType { get;set;}

        public Attachment()
        {
            this.Links = new List<Link>();
        }
    }
}
