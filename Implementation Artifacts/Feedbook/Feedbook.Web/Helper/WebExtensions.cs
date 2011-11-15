using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using log4net;
using System.Web.Security;

namespace VI.TalentExperts.Helper
{
    public static class WebExtensions
    {
        public static string GetUserName(this Page page)
        {
            return page.Session[Constant.Session.UserName] as string;
        }

        public static bool ValidateSession(this Page page)
        {
            if (page.GetUserName() == null)
            {
                FormsAuthentication.RedirectToLoginPage("message=" + HttpUtility.UrlEncode(Constant.Message.SessionExpired));
                return false;
            }

            return true;
        }

        public static void RedirectToErrorPage(this Page page)
        {
            page.Response.Redirect("~/ErrorPage.aspx", false);
        }

        public static void LogAndRedirectToErrorPage(this Page page, Exception exception)
        {
            ILog logger = LogManager.GetLogger(page.GetType());
            logger.Error(exception);
            page.RedirectToErrorPage();
        }
    }
}