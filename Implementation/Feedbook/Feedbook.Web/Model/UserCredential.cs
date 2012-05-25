using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using VI.TalentExperts.Helper;
using System.Text;
using CrystalMapper.Data;

namespace VI.TalentExperts.Model
{
    public partial class UserCredential
    {
        private static readonly MD5CryptoServiceProvider MD5Provider = new MD5CryptoServiceProvider();
    
        public static bool Authenticate(string userNameOrEmail, string password)
        {
            VIGuard.CheckNullOrWhiteSpace(userNameOrEmail, "Please provide user name or email address");
            VIGuard.CheckNullOrWhiteSpace(password, "Please provide login password");

            var passwordHash = ComputeHash(password);
            using (var dataContext = Constant.GetDataContext())
            {
                var users = from user in User.Query(dataContext)
                            join credential in UserCredential.Query(dataContext) on user.UserName equals credential.UserName
                            where (user.UserName == userNameOrEmail || user.Email == userNameOrEmail)
                            && credential.PasswordHash == passwordHash
                            select user;

                return users.Count() == 1;
            }
        }

        public static bool ChangePassword(string userName, string currentPassword, string newPassword)
        {
            var currentPasswordHash = ComputeHash(currentPassword);
            using (var dataContext = Constant.GetDataContext())
            {
                dataContext.BeginTransaction(System.Data.IsolationLevel.RepeatableRead);
                var credential = UserCredential.Query(dataContext).FirstOrDefault(u => u.UserName == userName && u.PasswordHash == currentPasswordHash);
                if (credential != null)
                {
                    credential.PasswordHash = ComputeHash(newPassword);
                    credential.UpdatedOn = DateTime.Now;
                    credential.UpdatedBy = userName;
                    return credential.Update(dataContext);
                }
            }

            return false;
        }

        public static string ComputeHash(string password)
        {
            return Convert.ToBase64String(MD5Provider.ComputeHash(Encoding.Unicode.GetBytes(password)));
        }
    }
}