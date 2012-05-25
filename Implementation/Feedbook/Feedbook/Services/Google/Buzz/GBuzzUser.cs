using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreSystem.Util;
using Feedbook.Specifications.PortableContacts;

namespace Feedbook.Services.Google.Buzz
{
    internal class GBuzzUser : ISocialUser
    {
        private Contact contact;

        public GBuzzUser(Contact contact)
        {
            Guard.CheckNull(contact, "GBuzzUser(contact)");
            this.contact = contact;
        }

        #region IFriend Members

        public string UserName
        {
            get { return this.contact.Id; }
        }

        public string Name
        {
            get { return this.contact.DisplayName; }
        }

        public string Url
        {
            get { return this.contact.ProfileUrl; }
        }

        public string ProfileImageUrl
        {
            get
            {
                if (this.contact.Photos != null && this.contact.Photos.Length > 0)
                    return this.contact.Photos.Last().Url;
                return null;
            }
        }

        public int Followers { get { return 0; } }

        public int Followings { get { return 0; } }

        #endregion
    }
}
