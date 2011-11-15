using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreSystem.Util;
using Feedbook.Specifications.Twitter;

namespace Feedbook.Services.Twitter
{
    internal class TwitterSocialUser : ISocialUser
    {
        private TwitterUser twitterUser;

        public TwitterSocialUser(TwitterUser user)
        {
            Guard.CheckNull(user, "TwitterSocialUser(user)");
            this.twitterUser = user;
        }

        #region IFriend Members

        public string UserName
        {
            get { return this.twitterUser.ScreenName; }
        }

        public string Name
        {
            get { return this.twitterUser.Name; }
        }

        public string Url
        {
            get { return this.twitterUser.Website; }
        }

        public string ProfileImageUrl
        {
            get { return this.twitterUser.ProfileImageLocation; }
        }

        public bool IsProtected
        {
            get { return this.twitterUser.IsProtected; }
        }

        public int Followers
        {
            get { return (int)this.twitterUser.NumberOfFollowers; }
        }

        public int Followings
        {
            get { return (int)this.twitterUser.NumberOfFriends; }
        }

        #endregion
    }
}
