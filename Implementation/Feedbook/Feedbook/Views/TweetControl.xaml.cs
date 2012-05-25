using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Feedbook.Services.Bitly;
using System.Threading;
using Feedbook.Helper;
using System.Net;
using Feedbook.Services.Twitter;
using Feedbook.Model;
using CoreSystem.RefTypeExtension;

namespace Feedbook.Views
{
    /// <summary>
    /// Interaction logic for TweetControl.xaml
    /// </summary>
    internal partial class TweetControl : UserControl
    {
        public TweetControl()
        {
            this.InitializeComponent();
        }

        public string Tweet
        {
            set
            {
                this.TweetBox.Text = value ?? string.Empty;
                this.TweetBox.SelectionStart = this.TweetBox.Text.Length;
                this.TweetBox.Focus();
            }
        }

        private void CloseTweetBoxButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.TweetBox.Text = string.Empty;
            this.TweetBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
            this.TweetBox.Height = 25;
            this.CloseTweetBoxButton.Visibility
             = this.RemainingCharTextBlock.Visibility
             = this.BottonGrid.Visibility
             = Visibility.Collapsed;
        }

        private void SendButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var accounts = new List<Account>();
            foreach (var selectedAccount in this.AccountListBox.SelectedItems)
            {
                if (selectedAccount as Account != null)
                    accounts.Add((Account)selectedAccount);
            }

            var tweet = this.TweetBox.Text;
            if (accounts.Count > 0 && !string.IsNullOrWhiteSpace(tweet))
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(
                    (object o) =>
                    {
                        try
                        {
                            foreach (var account in accounts)
                                TwitterService.Update(account.Token, account.TokenSecret, tweet);

                            this.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                this.TweetBox.Text = string.Empty;
                                this.TweetBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                                //this.Notify("Status has neen updated!");
                            }));
                        }
                        catch (Exception ex)
                        {
                            this.BeginInvoke(() => this.TwitterException("WOops! error occurred while sending tweet update on Twitter ...", ex));
                        }
                    }));
            }
        }

        private void TweetBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            this.RemainingCharTextBlock.Text = string.Format("{0}", (this.TweetBox.Text != null ? 140 - this.TweetBox.Text.Length : 140));

            if (this.AutoShortenUrlCheckBox.IsChecked == true)
            {
                try
                {
                    int caretIndex = this.TweetBox.SelectionStart;

                    if (caretIndex > 0)
                    {
                        string tweet = this.TweetBox.Text.Substring(0, caretIndex);
                        Uri uri = Util.GetLastUri(tweet);
                        if (uri != null && uri.Host != "bit.ly")
                        {
                            string uriString = uri.OriginalString;                            
                            ThreadPool.QueueUserWorkItem(new WaitCallback(
                                (object o) =>
                                {
                                    try
                                    {
                                        BitlyResponse response = BitlyService.ShortenUrl(uriString);
                                        if (response != null && response.StatusCode == 200
                                            && response.Data != null
                                            && !string.IsNullOrEmpty(response.Data.ShortUrl)
                                            && uriString.Length > response.Data.ShortUrl.Length)
                                        {
                                            this.Dispatcher.BeginInvoke(new Action(
                                                () =>
                                                {
                                                    try
                                                    {
                                                        if (this.TweetBox.Text != null)
                                                        {
                                                            int index = tweet.LastIndexOf(uriString);

                                                            if (index != -1)
                                                            {
                                                                this.TweetBox.Text = this.TweetBox.Text
                                                                                           .Remove(index, uriString.Length)
                                                                                           .Insert(index, response.Data.ShortUrl);
                                                                this.TweetBox.SelectionStart = index + response.Data.ShortUrl.Length + 1;
                                                            }
                                                        }
                                                    }
                                                    catch { }
                                                }));
                                        }

                                    }
                                    catch (Exception ex)
                                    {                                        
                                        var webException = ex as WebException;
                                        if (webException == null || webException.Response != null)
                                            this.Log("Failed to auto short url", ex);
                                    }
                                }));
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.Log("Error while shorting url", ex.AddData("Tweet", this.TweetBox.Text));
                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.AccountListBox.ItemsSource = DataStore.TwitterAccounts;
            this.AccountListBox.SelectedItem = DataStore.TwitterAccounts.FirstOrDefault(a => a.IsDefault) ?? DataStore.TwitterAccounts.FirstOrDefault();
        }

        private void TweetBox_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            this.TweetBox.Height = 50;
            this.CloseTweetBoxButton.Visibility
             = this.RemainingCharTextBlock.Visibility
             = this.BottonGrid.Visibility
             = Visibility.Visible;
        }

        private void TweetBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.TweetBox.Text) && !this.IsMouseOver)
            {
                this.TweetBox.Height = 25;
                this.CloseTweetBoxButton.Visibility
                 = this.RemainingCharTextBlock.Visibility
                 = this.BottonGrid.Visibility
                 = Visibility.Collapsed;
            }
        }
    }
}