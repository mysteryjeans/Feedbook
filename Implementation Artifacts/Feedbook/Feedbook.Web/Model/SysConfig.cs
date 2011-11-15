using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VI.TalentExperts.Helper;
using CrystalMapper.Data;

namespace VI.TalentExperts.Model
{
    public partial class SysConfig
    {
        public static string EmailAddress
        {
            get
            {
                var emailAddress = GetConfig(Constant.SysConfig.EmailAddress);
                if (emailAddress != null && !string.IsNullOrWhiteSpace(emailAddress)) return emailAddress;

                throw new VIException("Email address is not setup, please define email address in system settings");
            }
        }

        public static string SMTPServer
        {
            get
            {
                var smtpServer = GetConfig(Constant.SysConfig.SMTPServer);
                if (smtpServer != null && !string.IsNullOrWhiteSpace(smtpServer))
                    return smtpServer;

                throw new VIException("SMTP server address is not setup, please define SMTP address in system settings");
            }
        }

        public static string ActivationEmailSubject
        {
            get
            {
                var subject = GetConfig(Constant.SysConfig.ActivationEmailSubject);
                if (subject != null && !string.IsNullOrWhiteSpace(subject))
                    return subject;

                throw new VIException("Email subject is not setup, please define email subject in system settings");
            }
        }

        public static string ActivationEmailBody
        {
            get
            {
                var body = GetConfig(Constant.SysConfig.ActivationEmailBody);
                if (body != null && !string.IsNullOrWhiteSpace(body))
                    return body;

                throw new VIException("Email body is not setup, please define email body in system settings");
            }
        }

        public static string EmailUserName
        {
            get { return GetConfig(Constant.SysConfig.EmailUserName); }
        }

        public static string EmailUserPassword
        {
            get { return GetConfig(Constant.SysConfig.EmailUserPassword); }
        }

        public static void SetupEmailSettings(string emailAddress, string smtpServer, string userName, string password)
        {
            VIGuard.CheckNullOrWhiteSpace(emailAddress, "Email address is required");
            VIGuard.CheckNullOrWhiteSpace(smtpServer, "SMTP server address is required");
         
            using (var dataContext = Constant.GetDataContext())
            {
                dataContext.BeginTransaction();

                SetConfig(dataContext, Constant.SysConfig.EmailAddress, emailAddress);
                SetConfig(dataContext, Constant.SysConfig.SMTPServer, smtpServer);
                SetConfig(dataContext, Constant.SysConfig.EmailUserName, userName);
                SetConfig(dataContext, Constant.SysConfig.EmailUserPassword, password);

                dataContext.CommitTransaction();
            }
        }

        public static string GetConfig(string parameter)
        {
            var config = SysConfig.Query().FirstOrDefault(c => c.ParameterName == parameter);
            return config != null ? config.ParameterValue : null;
        }

        public static void SetConfig(string parameter, string value)
        {
            using (var dataContext = Constant.GetDataContext())
            {
                SetConfig(dataContext, parameter, value);
            }
        }

        public static void SetConfig(DataContext dataContext, string parameter, string value)
        {
            var config = SysConfig.Query(dataContext).FirstOrDefault(c => c.ParameterName == parameter);
            if (config == null)
                config = new SysConfig { ParameterName = parameter, ParameterValue = value };
            else
                config.ParameterValue = value;

            config.Save(dataContext);
        }
    }
}