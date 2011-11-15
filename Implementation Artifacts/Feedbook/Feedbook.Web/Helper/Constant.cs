using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CrystalMapper.Data;

namespace VI.TalentExperts.Helper
{
    public static class Constant
    {
        public class Default
        {
            public const string ApplicationName = "Talent Experts";

            public const int PasswordLength = 6;
        }

        public class Message
        {        
            public const string SessionExpired = "Your session has expired, please relogin!";

            public static string RegistrationSucessful = "Thank you! for registering with Talent Experts! you will receive an account activation email shortly.";
        }

        public static class Session
        {
            public const string UserName = "UserName";
        }

        public class SysConfig
        {
            public const string EmailAddress = "Email-Address";

            public const string SMTPServer = "SMTP-Server";

            public const string EmailUserName = "Email-User-Name";

            public const string EmailUserPassword = "Email-User-Password";

            public const string ActivationEmailBody = "Activation-Email-Body";

            public const string ActivationEmailSubject = "Activation-Email-Subject";
        }

        public static DataContext GetDataContext()
        {
            return new DataContext();
        }
    }
}