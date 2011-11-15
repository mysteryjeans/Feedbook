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
using System.Windows.Shapes;
using CoreSystem.RefTypeExtension;
using Feedbook.Helper;
using Feedbook.Model;
using Feedbook.Services;
using System.Collections.ObjectModel;
using System.Threading;
using System.Diagnostics;
using System.Web;
using Feedbook.Services.Twitter;
using System.Net;
using Feedbook.Specifications.Twitter;
using System.Collections.Specialized;

namespace Feedbook.Views
{
    internal partial class Twitter
    {
        public Twitter()
        {
            this.InitializeComponent();
            this.AddAccountMessage.Visibility = DataStore.TwitterAccounts.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            this.Loaded += (sender, e) => DataStore.TwitterAccounts.CollectionChanged += new NotifyCollectionChangedEventHandler(OnTwitterAccountsCollectionChanged);
            this.Unloaded += (sender, e) => DataStore.TwitterAccounts.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnTwitterAccountsCollectionChanged);
        }

        private void OnTwitterAccountsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.AddAccountMessage.Visibility = DataStore.TwitterAccounts.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            if (this.ListBox.ItemsSource as IEnumerable<Account> == null)
                this.RefreshCollectionSources();
        }

        private void AddAccount_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            new TwitterAccount() { Owner = Window.GetWindow(this) }.ShowDialog();
        }

        private void RefreshCollectionSources()
        {
            CompositeCollection collection = new CompositeCollection();
            DataStore.TwitterAccounts.ForEach(a => collection.Add(new CollectionContainer { Collection = a.Channels }));
            this.ListBox.ItemsSource = collection;
        }

        private void RadioButton_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            var radioButton = e.OriginalSource as RadioButton;
            if (radioButton != null)
            {
                //int milliseconds = (this.ListBox.ItemsSource != null) ? 200 : 0;
                //this.ListBox.HideSlow(milliseconds);
                this.BeginInvoke(() =>
                {
                    switch (radioButton.Tag + string.Empty)
                    {
                        case "Messages":
                            this.RefreshCollectionSources();
                            break;
                        case "Profiles":
                            this.ListBox.ItemsSource = DataStore.TwitterAccounts;
                            break;
                    }

                }, 50);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.MessagesRadioButton.IsChecked = true;
        }

        private void DeleteChannel_Click(object sender, RoutedEventArgs e)
        {
            var rmvChannel = Util.DataContextAs<Channel>(e.OriginalSource);
            if (rmvChannel != null)
            {
                var channelAccount = (from account in DataStore.TwitterAccounts
                                      from channel in account.Channels
                                      where rmvChannel == channel
                                      select account).FirstOrDefault();

                if (channelAccount != null)
                    channelAccount.Channels.Remove(rmvChannel);
            }
        }

        private void MenuOpen_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.ChannelMenu.Items.Clear();
            try
            {

                var channels = from account in DataStore.TwitterAccounts
                               from channel in SocialService.GetSocialService(account).GetFeeds()
                               where !account.Channels.Any(c => c.ChannelId == channel.ChannelId)
                               select new { Account = account, Channel = channel };

                var accounts = from pair in channels
                               group pair.Channel by pair.Account into g
                               select new { Account = g.Key, Channels = g };

                var isFirst = true;
                foreach (var account in accounts)
                {
                    if (!isFirst)
                        this.ChannelMenu.Items.Add(new Separator());

                    foreach (var channel in account.Channels)
                        this.ChannelMenu.Items.Add(GetMenuItem(account.Account, channel));

                    isFirst = false;
                }

                this.ChannelMenu.IsSubmenuOpen = true;
            }
            catch (Exception ex)
            {
                this.LogAndShowError("WOops! error occurred while formulating channel list", ex);
            }
        }

        private MenuItem GetMenuItem(Account account, Channel channel)
        {
            var menuItem = new MenuItem { Header = channel.Title };
            menuItem.Click += (sender, e) =>
            {
                account.Channels.Add(channel);
                SocialService.GetSocialService(account).AsyncUpdate(channel);
            };

            return menuItem;
        }

        private void RemoveAccountButton_Click(object sender, RoutedEventArgs e)
        {
            var account = Util.DataContextAs<Account>(e.OriginalSource);
            if (account != null)
                DataStore.TwitterAccounts.Remove(account);
        }

        private void UnfollowButton_Click(object sender, RoutedEventArgs e)
        {
            var friends = new ObservableCollection<ISocialUser>();
            var button = e.OriginalSource as DependencyObject;
            var friend = Helper.Util.DataContextAs<ISocialUser>(e.OriginalSource);

            if (button != null && friend != null)
            {
                var listBox = button.GetVisualAncestors()
                                    .OfType<ListBox>()
                                    .FirstOrDefault();

                var account = button.GetVisualAncestors()
                                    .OfType<FrameworkElement>()
                                    .Where(element => element.DataContext is Account)
                                    .Select(element => (Account)element.DataContext)
                                    .FirstOrDefault();

                if (listBox != null && account != null)
                {
                    listBox.ItemsSource = friends;
                    ThreadPool.QueueUserWorkItem(new WaitCallback(
                        (object o) =>
                        {
                            try
                            {
                                ISocialService service = SocialService.GetSocialService(account);
                                service.UnFollow(new Person { UserId = friend.UserName });
                                var friendList = service.GetFriends();
                                this.BeginInvoke(() => friends.AddRange(friendList));
                            }
                            catch (Exception ex)
                            {
                                this.Dispatcher.BeginInvoke(new Action(() => this.TwitterException("WOops! error occurred while removing user from followings.", ex)));
                            }
                        }));
                }

            }
        }

        private void ContentPresenter_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            e.Handled = true;

            try
            {
                switch (e.Target)
                {
                    case "TwitterSearch":
                        {
                            var account = this.GetAccount(e);
                            if (account != null && e.Uri.ToString().Trim().Length > 0 && account != null)
                            {
                                string queryUrl = "http://search.twitter.com/search.atom?q=" + HttpUtility.UrlEncode(e.Uri.ToString());
                                var channel = new Channel { Title = e.Uri.ToString() + " / " + account.UserName, DownloadUrl = queryUrl, ChannelId = queryUrl };
                                var existingChannel = account.Channels.FirstOrDefault(c => c.ChannelId == channel.ChannelId);
                                if (existingChannel != null)
                                    account.Channels.Remove(existingChannel);
                                SocialService.GetSocialService(account).AsyncUpdate(channel);
                                account.Channels.Add(channel);
                            }
                        }
                        break;
                    case "TwitterProfile":
                        {
                            var account = this.GetAccount(e);
                            if (account != null && e.Uri.ToString().Trim().Length > 0 && account != null)
                                this.ShowProfile(account, e.Uri.ToString());
                        }
                        break;
                    default:
                        Process.Start(e.Uri.ToString());
                        break;
                }
            }
            catch (Exception ex)
            {
                this.LogAndShowError("WOops! occurred occurred while preforming navigation request.", ex);
            }
        }

        private void ReplyButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var feed = Util.DataContextAs<Feed>(e.OriginalSource);

            if (feed != null && feed.Author != null)
                this.TweetControl.Tweet = "@" + feed.Author.UserId + " ";
        }

        private void RetweetButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Account account;
            var feed = Util.DataContextAs<Feed>(e.OriginalSource);
            if (feed != null && (account = this.GetAccount(feed)) != null)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(
                    (object o) =>
                    {
                        try
                        {
                            var service = SocialService.GetSocialService(account);
                            service.Share(feed);
                            this.BeginInvoke(() => this.Notify("Updated has been retweeted!"));
                        }
                        catch (Exception ex)
                        {
                            this.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                if (ex.HttpStatusCode() == HttpStatusCode.Forbidden)
                                    FBMessageBox.Show("Cannot retweet this status, it may be protected or retweet by you earlier!");
                                else
                                    this.TwitterException("WOops! error occurred while retweet on twitter ...", ex);
                            }));
                        }
                    }));
            }
        }

        private void FollowButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var feed = Util.DataContextAs<Feed>(e.OriginalSource);
            var account = GetAccount(feed);
            if (feed != null && account != null && feed.Author != null)
                Follow(account, feed.Author.UserId);
        }

        private void ProfileButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Account account;
            var feed = Util.DataContextAs<Feed>(e.OriginalSource);
            if (feed != null && (account = GetAccount(feed)) != null && feed.Author != null && !string.IsNullOrEmpty(feed.Author.UserId))
                this.ShowProfile(account, feed.Author.UserId);
        }

        private void ShowProfile(Account account, string screenName)
        {
            this.ProfileGrid.Hide(Constants.Default.HideMilliseconds);
            DateTime hideStartTime = DateTime.Now;
            ThreadPool.QueueUserWorkItem(new WaitCallback(
                (object o) =>
                {
                    try
                    {
                        var user = TwitterService.Show(screenName, account.Token, account.TokenSecret);

                        var wait = hideStartTime.AddMilliseconds(Constants.Default.HideMilliseconds) - DateTime.Now;
                        if (wait.TotalMilliseconds > 0)
                            Thread.Sleep((int)wait.TotalMilliseconds + 1);

                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            this.ProfileGrid.DataContext = user;
                            this.ProfileGrid.Tag = account;
                            this.ProfileGrid.Show(Constants.Default.ShowMilliseconds);

                        }));
                    }
                    catch (Exception ex)
                    {
                        this.Dispatcher.BeginInvoke(new Action(() => this.TwitterException("WOops! error occurred while retrieving profile ...", ex)));
                    }
                }));
        }

        private void FollowMe_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var user = this.ProfileGrid.DataContext as TwitterUser;
            var account = this.ProfileGrid.Tag as Account;
            if (user != null && account != null)
                Follow(account, user.ScreenName);
        }

        private void CloseProfile_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.ProfileGrid.Hide(200);
            this.BeginInvoke(() => this.ProfileGrid.Visibility = Visibility.Collapsed, 250);
        }

        private void TwitterSearchButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string searchWord;
            var account = GetAccount();
            if (this.TwitterSearchTextBox.Text != null && (searchWord = this.TwitterSearchTextBox.Text).Trim().Length > 0 && account != null)
            {

                string queryUrl = "http://search.twitter.com/search.atom?q=" + HttpUtility.UrlEncode(searchWord);
                var channel = new Channel { Title = searchWord + " / " + account.UserName, DownloadUrl = queryUrl, ChannelId = queryUrl };
                var existingChannel = account.Channels.FirstOrDefault(c => c.ChannelId == channel.ChannelId);
                if (existingChannel != null)
                    account.Channels.Remove(existingChannel);
                account.Channels.Add(channel);
                SocialService.GetSocialService(account).AsyncUpdate(channel);
                this.TwitterSearchTextBox.Text = string.Empty;
            }
        }

        private void AccountIsDefault_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            var checkBox = e.OriginalSource as CheckBox;
            var account = Util.DataContextAs<Account>(e.OriginalSource);
            if (checkBox != null && checkBox.IsChecked == true && account != null)
            {
                DataStore.TwitterAccounts
                         .Where(a => a != account)
                         .ForEach(a => a.IsDefault = false);
            }
        }

        private void Follow(Account account, string screenName)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(
                (object o) =>
                {
                    try
                    {
                        var service = SocialService.GetSocialService(account);
                        service.Follow(new Person { UserId = screenName });
                        this.BeginInvoke(() => this.Notify(string.Format("Now following @{0}!", screenName)));
                    }
                    catch (Exception ex)
                    {
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            if (ex.HttpStatusCode() == HttpStatusCode.Forbidden)
                                FBMessageBox.Show("You may already following this user!");
                            else
                                this.TwitterException("WOops! error occurred while following this user ...", ex);
                        }));
                    }
                }));
        }

        private Account GetAccount(RoutedEventArgs e)
        {
            var feed = Util.DataContextAs<Feed>(e.OriginalSource);
            if (feed != null) return GetAccount(feed);

            return DataStore.TwitterAccounts.FirstOrDefault();
        }

        private Account GetAccount()
        {
            return DataStore.TwitterAccounts.FirstOrDefault(a => a.IsDefault) ?? DataStore.TwitterAccounts.FirstOrDefault();
        }

        private Account GetAccount(Feed feed)
        {
            return (from a in DataStore.TwitterAccounts
                    from c in a.Channels
                    where c.Feeds.Contains(feed)
                    select a).FirstOrDefault();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var feed = Util.DataContextAs<Feed>(e.OriginalSource);
            if (feed != null)
            {
                try { Clipboard.SetText(feed.EncodedDescription, TextDataFormat.Text); }
                catch (Exception ex) { this.LogAndShow("Failed to copy content into clipboard", ex); }
            }
        }
    }
}