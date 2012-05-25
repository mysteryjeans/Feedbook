using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Feedbook.Web.Helper
{
    public static class FBGuard
    {
        public static void CheckNullOrEmpty(string value, string message)
        {
            if (string.IsNullOrEmpty(value))
                throw new FBException(message);
        }

        public static void CheckNullOrWhiteSpace(string value, string message)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new FBException(message);
        }
    }
}