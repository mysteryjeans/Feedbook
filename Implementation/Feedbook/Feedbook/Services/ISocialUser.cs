using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Feedbook.Services
{
    public interface ISocialUser
    {
        string UserName { get; }

        string Name { get; }

        string Url { get; }

        string ProfileImageUrl { get; }

        int Followers { get; }

        int Followings { get; }
    }
}
