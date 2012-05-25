using System;
using System.Linq;
using Feedbook.Web.Helper;
using System.Web;

namespace Feedbook.Web.Model
{
    public partial class User
    {
        internal static void Activate(string userName, string verificationCode)
        {
            FBGuard.CheckNullOrWhiteSpace(userName, "User name is missing.");
            FBGuard.CheckNullOrWhiteSpace(verificationCode, "Verification code is missing.");

            using (var dataContext = Constant.GetDataContext())
            {
                dataContext.BeginTransaction(System.Data.IsolationLevel.RepeatableRead);
                var user = User.Query(dataContext).FirstOrDefault(u => u.UserName == userName);
                if (user == null)
                    throw new FBException("Account not found!");

                if (user.IsVerified)
                    throw new FBException("Your account is already activated!");

                if (user.VerificationCode != verificationCode)
                    throw new FBException("Invalid activation code!");

                user.IsVerified = true;
                user.Save(dataContext);

                dataContext.CommitTransaction();
            }
        }

        internal static User Update(string userName, string name, string email, string domain, string modifiedBy)
        {
            FBGuard.CheckNullOrWhiteSpace(userName, "Please provide user name.");
            FBGuard.CheckNullOrWhiteSpace(name, "Please provide your name.");
            FBGuard.CheckNullOrWhiteSpace(email, "Please provide email.");
            FBGuard.CheckNullOrWhiteSpace(domain, "Please provide domain.");

            name = name.Trim();
            email = email.Trim();
            domain = domain.Trim().ToLower();

            var now = DateTime.Now;
            using (var dataContext = Constant.GetDataContext())
            {
                if (User.Query(dataContext).Count(u => u.UserName != userName && u.email == email) > 0)
                    throw new FBException("Email address is already exists.");

                if (User.Query(dataContext).Count(u => u.UserName != userName && u.Domain == domain) > 0)
                    throw new FBException("Domain name is already exists.");

                var user = User.Query(dataContext).FirstOrDefault(u => u.UserName == userName);
                if (user == null)
                    throw new FBException("Account not found!");

                user.Name = name;
                user.Domain = domain;

                if (user.Email != email)
                {
                    user.Email = email;
                    user.IsVerified = false;
                    user.VerificationCode = GetVerificationCode();
                }

                user.Save(dataContext);

                if (!user.IsVerified)
                    user.SendActivationCode();

                return user;
            }
        }

        internal static void Register(string userName, string password, string name, string email, string domain)
        {
            FBGuard.CheckNullOrWhiteSpace(userName, "Please provide user name.");
            FBGuard.CheckNullOrWhiteSpace(password, "Please provide login password.");
            FBGuard.CheckNullOrWhiteSpace(name, "Please provide your name.");
            FBGuard.CheckNullOrWhiteSpace(email, "Please provide email.");
            FBGuard.CheckNullOrWhiteSpace(domain, "Please provide domain.");

            if (password.Length < Constant.Default.PasswordLength)
                throw new FBException(string.Format("Passwords are required to be a minimum of {0} characters in length.", Constant.Default.PasswordLength));

            name = name.Trim();
            email = email.Trim();
            domain = domain.Trim().ToLower();
            userName = userName.Trim().ToLower();

            var now = DateTime.Now;
            using (var dataContext = Constant.GetDataContext())
            {
                dataContext.BeginTransaction(System.Data.IsolationLevel.Serializable);

                if (User.Query(dataContext).Count(u => u.UserName == userName) > 0)
                    throw new FBException("User name is already exists.");

                if (User.Query(dataContext).Count(u => u.email == email) > 0)
                    throw new FBException("Email address is already exists.");

                if (User.Query(dataContext).Count(u => u.Domain == domain) > 0)
                    throw new FBException("Domain name is already exists.");

                var user = new User
                           {
                               Name = name,
                               Email = email,
                               Domain = domain,
                               IsVerified = false,
                               UserName = userName,
                               UpdatedOn = now,
                               UpdatedBy = userName,
                               CreatedOn = now,
                               CreatedBy = userName,
                               VerificationCode = GetVerificationCode()
                           };

                user.Save(dataContext);

                var credential = new UserCredential
                                 {
                                     UserRef = user,
                                     PasswordHash = UserCredential.ComputeHash(password),
                                     UpdatedOn = now,
                                     UpdatedBy = userName
                                 };

                credential.Save(dataContext);

                dataContext.CommitTransaction();

                user.SendActivationCode();
            }
        }

        private void SendActivationCode()
        {
            var body = SysConfig.ActivationEmailBody;
            body = body.Replace("@Name", this.Name)
                       .Replace("@ActivationUrl", string.Format("http://services.feedbook.org/Activation.aspx?username={0}&code={1}", HttpUtility.UrlEncode(this.UserName), HttpUtility.UrlEncode(this.VerificationCode)));

            Util.SendEmail(this.email, SysConfig.ActivationEmailSubject, body);
        }

        private static string GetVerificationCode()
        {
            return Guid.NewGuid().ToString();
        }
    }
}