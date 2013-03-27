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
using System.Threading;
using Feedbook.Model;
using Feedbook.Services;
using System.Net;
using CoreSystem.RefTypeExtension;
using Feedbook.Helper;
using Feedbook.Services.Security;
using Feedbook.Services.Google.Buzz;
using System.Windows.Navigation;

namespace Feedbook.Views
{
    /// <summary>
    /// Interaction logic for GBuzzAccount.xaml
    /// </summary>
    internal partial class GBuzzAccount : Window
    {
        private string token;
        private string tokenSecret;

        public GBuzzAccount()
        {
            InitializeComponent();
        }

        private void webBrowser_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.Uri != null && e.Uri.OriginalString.Contains(Constants.WEBSITE_URL))
            {
                try
                {
                    var queryString = OAuthHelper.ParseQueryString(e.Uri.Query);
                    var verificationCode = queryString[OAuthHelper.OAUTH_VERIFIER];

                    ThreadPool.QueueUserWorkItem(new WaitCallback(
                        (object o) =>
                        {
                            try
                            {
                                var buzzService = new GBuzzService();
                                buzzService.GetAccessToken(token, tokenSecret, verificationCode, out token, out tokenSecret);
                                var contact = GBuzzService.GetUserProfile(this.token, this.tokenSecret);

                                var account = new Account { Token = this.token, TokenSecret = this.tokenSecret, AccountType = Model.AccountType.Google, UserName = contact.Id, FullName = contact.DisplayName };
                                var service = new GBuzzSocialService(account);
                                account.Channels.AddRange(service.GetFeeds());
                                if (contact.Photos.Length > 0)
                                    account.ImageUrl = contact.Photos.Last().Url;


                                this.Dispatcher.BeginInvoke(new Action(
                                    () =>
                                    {
                                        var existingAccount = DataStore.GBuzzAccounts.FirstOrDefault(a => a.UserName == account.UserName);
                                        account.Channels.ForEach(c => service.AsyncUpdate(c));
                                        account.IsDefault = !DataStore.GBuzzAccounts.Any(a => a.IsDefault);
                                        this.DialogResult = true;
                                        this.Close();

                                        if (existingAccount != null)
                                        {
                                            existingAccount.Token = account.Token;
                                            existingAccount.TokenSecret = account.TokenSecret;
                                            existingAccount.IsDefault = account.IsDefault;
                                            this.Notify("{0} is refreshed successfully!", existingAccount.FullName);
                                        }
                                        else
                                        {
                                            DataStore.GBuzzAccounts.Add(account);
                                            this.Notify("{0} is added successfully!", account.FullName);
                                        }
                                    }));

                            }
                            catch (WebException webEx)
                            {
                                this.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    HttpWebResponse response = webEx.Response as HttpWebResponse;
                                    if (response != null && response.StatusCode == HttpStatusCode.Unauthorized)
                                        FBMessageBox.Show("Login failer on Google Buzz ...!");
                                    else
                                        FBMessageBox.Show("WOops! failed to successfully obtain verfication code");
                                }));
                            }
                            catch (Exception ex)
                            {
                                this.Dispatcher.BeginInvoke(new Action(() => this.LogAndShowError("WOops! failed to successfully obtain verfication code", ex)));
                            }
                        }));
                }
                catch (Exception ex)
                {
                    this.LogAndShowError("WOops! Authentication cannot be completed, unable to obtain verification code", ex);
                }
            }

            // this.progressBar.IsIndeterminate = true;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //this.progressBar.IsIndeterminate = true;
            ThreadPool.QueueUserWorkItem(new WaitCallback(
                (object o) =>
                {
                    try
                    {
                        var buzzService = new GBuzzService();
                        buzzService.GetRequestToken(out token, out tokenSecret);
                        string url = buzzService.GetAuthorizationUrl(token);
                        this.Dispatcher.Invoke(new Action(() => this.webBrowser.Source = new Uri(url)));
                    }
                    catch (Exception ex)
                    {
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            this.LogAndShowError("WOops! error occurred while loading google buzz authentication", ex);
                            this.Close();
                        }));
                    }
                }));
        }

        private void CloseButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
