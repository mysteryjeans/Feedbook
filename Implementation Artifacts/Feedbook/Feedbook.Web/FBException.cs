using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Feedbook.Web
{
    public class FBException : Exception
    {
        public FBException(string message)
            : base(message)
        { }

        public FBException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}