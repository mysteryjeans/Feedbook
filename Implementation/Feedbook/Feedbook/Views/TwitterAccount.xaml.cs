using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Feedbook.Services.Twitter;
using System.Threading;
using Feedbook.Model;
using Feedbook.Services;
using System.Net;
using CoreSystem.RefTypeExtension;
using Feedbook.Helper;

namespace Feedbook.Views
{
    /// <summary>
    /// Interaction logic for TwitterAccount.xaml
    /// </summary>
    internal partial class TwitterAccount : Window
    {
        public TwitterAccount()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string username;
            string password;

            if (this.UserNameTextBox.Text != null
                && this.PasswordTextBox.Password != null
                && (username = this.UserNameTextBox.Text.Trim()).Length > 0
                && (password = this.PasswordTextBox.Password.Trim()).Length > 0)
            {
                string token;
                string tokenSecret;

                this.ProgressBar.IsIndeterminate = true;
                ThreadPool.QueueUserWorkItem(new WaitCallback(
                    (object o) =>
                    {
                        try
                        {
                            TwitterService.GetAccessToken(username, password, out token, out tokenSecret);
                            var contact = TwitterService.Show(username);
                            var account = new Account { FullName = contact.Name, UserName = username, AccountType = AccountType.Twitter, Token = token, TokenSecret = tokenSecret, ImageUrl = contact.ProfileImageLocation };

                            var service = SocialService.GetSocialService(account);
                            account.Channels.AddRange(service.GetFeeds());

                            this.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                this.ProgressBar.IsIndeterminate = false;
                                var existingAccount = DataStore.TwitterAccounts.FirstOrDefault(a => a.UserName == account.UserName && a.AccountType == AccountType.Twitter);

                                account.Channels.ForEach(c => service.AsyncUpdate(c));

                                account.IsDefault = !DataStore.TwitterAccounts.Any(a => a.IsDefault);
                                this.DialogResult = true;
                                this.Close();
                                if (existingAccount != null)
                                {
                                    existingAccount.Token = account.Token;
                                    existingAccount.TokenSecret = account.TokenSecret;
                                    existingAccount.IsDefault = account.IsDefault;
                                    this.Notify("@{0} is refreshed successfully!", existingAccount.UserName);
                                }
                                else
                                {
                                    DataStore.TwitterAccounts.Add(account);
                                    this.Notify("@{0} is added successfully!", account.UserName);
                                }                              
                            }));
                        }
                        catch (WebException webEx)
                        {
                            this.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                this.ProgressBar.IsIndeterminate = false;
                                HttpWebResponse response = webEx.Response as HttpWebResponse;
                                if (response != null && response.StatusCode == HttpStatusCode.Unauthorized)
                                    FBMessageBox.Show("Twitter authentication failed...!");
                                else
                                    FBMessageBox.Show("WOops! error occurred while authenticating with twitter.");
                            }));
                        }
                        catch (Exception ex)
                        {
                            this.Dispatcher.BeginInvoke(new Action(() => this.LogAndShowError("WOops! error occurred while authenticating with twitter.", ex)));
                        }
                        finally
                        {
                            this.Dispatcher.BeginInvoke(new Action(() => this.ProgressBar.IsIndeterminate = false));
                        }
                    }));
            }
        }

        private void PasswordTextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.PasswordTextBox.Password))
                this.PasswordTextBox.ToolTip = "Password";
        }

        private void PasswordTextBox_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            this.PasswordTextBox.ToolTip = null;
        }

        private void CloseButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
