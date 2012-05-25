using System;
using System.Linq;
using System.Text;
using CoreSystem.Util;
using Feedbook.Model;
using System.Collections.Generic;
using Feedbook.Services.WebFeed;
using Feedbook.Services.Twitter;
using Feedbook.Services.Google.Buzz;

namespace Feedbook.Services
{
    internal abstract class SocialService : ISocialService
    {
        public Account Account { get; private set; }   

        public string UserId { get { return this.Account.UserName; } }

        public string Token { get { return this.Account.Token; } }

        public string TokenSecret { get { return this.Account.TokenSecret; } }

        public SocialService()
        { }

        public SocialService(Account account)
        {
            Guard.CheckNull(account, "SocialService(account)");
            this.Account = account;
        }

        #region IUserProfile Members

        public static ISocialService GetSocialService(Account account)
        {
            if (account == null)
                return new FeedSocialService();

            switch (account.AccountType)
            {
                case AccountType.Google:
                    return new GBuzzSocialService(account);
                case AccountType.Twitter:
                    return new TwitterSocialService(account);
                default:
                    throw new NotSupportedException(string.Format("Account type '{0}' is not supported for any social service", account.AccountType));
            }
        }

        public abstract FeedProcessor Processor { get; }

        public abstract bool IsReplySupported { get; }

        public abstract bool IsLikeSupported { get; }

        public abstract bool IsShareSupported { get; }

        public abstract bool IsFollowSupported { get; }

        public abstract ISocialUser GetUserProfile();

        public abstract ISocialUser[] GetFriends();

        public abstract IEnumerable<Channel> GetFeeds();

        public abstract void AsyncUpdate(Channel channel);

        public abstract Feed Post(string message, Attachment[] attachments);

        public abstract Feed Update(Feed feed, string message, Attachment[] attachments);
        
        public abstract Feed Reply(Feed feed, string comments);

        public abstract void Like(Feed feed);

        public abstract void Remove(Feed feed);

        public abstract void Share(Feed feed);

        public abstract void Follow(Person person);

        public abstract void UnFollow(Person person);

        #endregion
    }
}
