using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Feedbook.Helper
{
    internal static class ExceptionExtension
    {
        public static HttpStatusCode? HttpStatusCode(this Exception exception)
        {
            WebException webException;
            HttpWebResponse httpResponse;

            return (webException = exception as WebException) != null && (httpResponse = webException.Response as HttpWebResponse) != null
                   ? (HttpStatusCode?)httpResponse.StatusCode : null;
        }
    }
}
