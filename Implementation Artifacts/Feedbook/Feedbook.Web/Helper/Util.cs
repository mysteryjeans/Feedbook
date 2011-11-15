using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using VI.TalentExperts.Model;
using System.Net;

namespace VI.TalentExperts.Helper
{
    public static class Util
    {
        public static void SendEmail(string toEmail, string subject, string body)
        {
            SmtpClient client;

            string smtpServer = SysConfig.SMTPServer;

            if (smtpServer.Contains(":"))
            {
                var smtpAddParts = smtpServer.Split(':');
                int port;
                if (!int.TryParse(smtpAddParts[1], out port) || port < 1)
                    throw new VIException(string.Format("Invalid port: {0} number", smtpAddParts[1]));

                client = new SmtpClient(smtpAddParts[0], port);
            }
            else
                client = new SmtpClient(smtpServer);

            MailMessage message = new MailMessage();
            if (!string.IsNullOrWhiteSpace(SysConfig.EmailUserName) && !string.IsNullOrWhiteSpace(SysConfig.EmailUserPassword))
                client.Credentials = new NetworkCredential(SysConfig.EmailUserName, SysConfig.EmailUserPassword);

            message.To.Add(toEmail);
            message.From = new MailAddress(SysConfig.EmailAddress);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            client.Send(message);
        }
    }
}