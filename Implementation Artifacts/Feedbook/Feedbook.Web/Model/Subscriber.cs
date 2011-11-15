using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CrystalMapper.Linq;

using VI.TalentExperts.Helper;
using System.Net.Sockets;

namespace VI.TalentExperts.Model
{
    public partial class Subscriber
    {
        public static void Subscribe(params string[] emails)
        {
            if (emails == null && emails.Length == 0)
                throw new VIException("Please provide atleast one email address.");

            using (var context = Constant.GetDataContext())
            {
                context.BeginTransaction(System.Data.IsolationLevel.Serializable);

                var now = DateTime.Now;
                var subscriber = new Subscriber { UpdatedOn = now, UpdatedBy = "Subscriber", CreatedOn = now, CreatedBy = "Subscriber" };
                foreach (var email in emails)
                {
                    var subscriberEmail = new SubscriberEmail { Email = email.ToLower().Trim(), CreatedOn = now, CreatedBy = "Subscriber" };

                    if (SubscriberEmail.Query().Count(e => e.Email == subscriberEmail.Email) > 0)
                        throw new VIException(string.Format("Email address '{0}' is already subscribe in our database.", subscriberEmail.Email));

                    subscriber.subscriberEmails.Add(subscriberEmail);
                }

                subscriber.SaveChanges(context);

                context.CommitTransaction();
            }
        }

        public static int SendAlert(string subject, string message, string userName)
        {
            var alerts = 0;
            try
            {
                foreach (var subscriber in SubscriberEmail.Query())
                {
                    Util.SendEmail(subscriber.Email, subject, message);
                    alerts++;
                }
            }
            catch (SocketException ex)
            {
                switch (ex.SocketErrorCode)
                {
                    case SocketError.HostDown:
                        throw new VIException("SMTP server is down.");
                    case SocketError.HostNotFound:
                        throw new VIException("Invalid SMTP Mail Server, please vendor to define correct SMTP server.");
                    case SocketError.HostUnreachable:
                        throw new VIException("SMTP server is unreachable");
                    default:
                        throw new VIException(ex.Message, ex.InnerException);
                }
            }
            catch (Exception ex)
            {
                if (alerts > 0)
                    throw new VIException(string.Format("Error occurred while sending email, but {0} email(s) have been send sucessfully.", alerts), ex);

                throw new VIException("Error occurred while sending email.", ex);
            }

            return alerts;
        }
    }
}