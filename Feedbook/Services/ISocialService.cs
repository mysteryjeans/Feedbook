using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Feedbook.Model;
using Feedbook.Services.WebFeed;

namespace Feedbook.Services
{
    internal interface ISocialService 
    {
        bool IsReplySupported { get; }

        bool IsLikeSupported { get; }

        bool IsShareSupported { get; }

        bool IsFollowSupported { get; }

        Account Account { get; }

        FeedProcessor Processor { get; }

        ISocialUser GetUserProfile();

        ISocialUser[] GetFriends();

        IEnumerable<Channel> GetFeeds();

        void AsyncUpdate(Channel channel);

        Feed Post(string message, Attachment[] attachments);

        Feed Update(Feed feed, string message, Attachment[] attachments);

        void Remove(Feed feed);

        void Like(Feed feed);

        void Share(Feed feed);

        Feed Reply(Feed feed, string comments);

        void Follow(Person person);

        void UnFollow(Person person);
    }
}
