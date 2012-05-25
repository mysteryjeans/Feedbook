using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VI.TalentExperts.Helper
{
    public static class VIGuard
    {
        public static void CheckNullOrEmpty(string value, string message)
        {
            if (string.IsNullOrEmpty(value))
                throw new VIException(message);
        }

        public static void CheckNullOrWhiteSpace(string value, string message)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new VIException(message);
        }
    }
}