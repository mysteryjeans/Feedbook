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
using System.Net;
using Feedbook.Services.Google.Buzz;
using System.Collections.Specialized;

namespace Feedbook.Views
{
    internal partial class GoogleBuzz
    {
        public GoogleBuzz()
        {
            this.InitializeComponent();
            this.AddAccountMessage.Visibility = DataStore.GBuzzAccounts.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            this.Loaded += (sender, e) => DataStore.GBuzzAccounts.CollectionChanged += new NotifyCollectionChangedEventHandler(OnBuzzAccountsCollectionChanged);
            this.Unloaded += (sender, e) => DataStore.GBuzzAccounts.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnBuzzAccountsCollectionChanged);
        }

        private void OnBuzzAccountsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.AddAccountMessage.Visibility = DataStore.GBuzzAccounts.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            if (this.ListBox.ItemsSource as IEnumerable<Account> == null)
                this.RefreshCollectionSources();
        }

        private void AddAccount_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            new GBuzzAccount() { Owner = Window.GetWindow(this) }.ShowDialog();
        }

        private void RefreshCollectionSources()
        {
            CompositeCollection collection = new CompositeCollection();
            DataStore.GBuzzAccounts.ForEach(a => collection.Add(new CollectionContainer { Collection = a.Channels }));
            this.ListBox.ItemsSource = collection;
        }

        private void RadioButton_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            var radioButton = e.OriginalSource as RadioButton;
            if (radioButton != null)
            {
                int milliseconds = (this.ListBox.ItemsSource != null) ? 200 : 0;

                this.ListBox.HideSlow(milliseconds);
                this.BeginInvoke(() =>
                {
                    this.ListBox.ShowSlow(milliseconds + 100);
                    switch (radioButton.Tag + string.Empty)
                    {
                        case "Messages":
                            this.RefreshCollectionSources();
                            break;
                        case "Profiles":
                            this.ListBox.ItemsSource = DataStore.GBuzzAccounts;
                            break;
                    }
                }, milliseconds + 50);
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
                var channelAccount = (from account in DataStore.GBuzzAccounts
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

                var channels = from account in DataStore.GBuzzAccounts
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
                this.LogAndShowError("WOops! something goes wrong while rendering menu", ex);
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
                DataStore.GBuzzAccounts.Remove(account);
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
                                this.Dispatcher.BeginInvoke(new Action(() => this.LogAndShowError("WOops! error occurred while removing user from followings list", ex)));
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
                    case "GoogleSearch":
                        {
                            var account = this.GetAccount(e);
                            if (account != null && e.Uri.ToString().Trim().Length > 0 && account != null)
                            {
                                string queryUrl = "https://www.googleapis.com/buzz/v1/activities/search?q=" + HttpUtility.UrlEncode(e.Uri.ToString());
                                var channel = new Channel { Title = e.Uri.ToString() + " / " + account.UserName, DownloadUrl = queryUrl, ChannelId = queryUrl };
                                var existingChannel = account.Channels.FirstOrDefault(c => c.ChannelId == channel.ChannelId);
                                if (existingChannel != null)
                                    account.Channels.Remove(existingChannel);
                                SocialService.GetSocialService(account).AsyncUpdate(channel);
                                account.Channels.Add(channel);
                            }
                        }
                        break;
                    default:
                        Process.Start(e.Uri.ToString());
                        break;
                }
            }
            catch (Exception ex)
            {
                ex.AddData("URI", e.Uri);
                this.LogAndShowError("WOops! unable to open specified url", ex);
            }
        }

        private void CommentButton_Click(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;
            if (button != null)
            {
                var border = button.FindName("commentControl") as Border;
                var commentBox = button.FindName("commentBox") as TextBox;
                if (border != null && commentBox != null)
                {
                    border.Show();
                    commentBox.Focus();
                }
            }
        }

        private void FollowButton_Click_1(object sender, RoutedEventArgs e)
        {
            Account account;
            var feed = Util.DataContextAs<Feed>(e.OriginalSource);

            if (feed != null && (account = this.GetAccount(feed)) != null)
            {
                //this.ProgressBar.IsIndeterminate = true;
                ThreadPool.QueueUserWorkItem(new WaitCallback(
                    (object o) =>
                    {
                        try
                        {
                            var service = SocialService.GetSocialService(account);
                            service.Follow(feed.Author);
                            this.BeginInvoke(() => this.Notify(string.Format("Now following @{0}!", feed.Author.Name)));
                        }
                        catch (Exception ex)
                        {
                            this.Dispatcher.BeginInvoke(new Action(() => this.GBuzzException("WOops! error occurred while following this user ...", ex)));
                        }
                    }));
            }
        }

        private void LikeButton_Click(object sender, RoutedEventArgs e)
        {
            Account account;
            var feed = Util.DataContextAs<Feed>(e.OriginalSource);

            if (feed != null && (account = this.GetAccount(feed)) != null)
            {                
                var service = new GBuzzSocialService(account, this.Dispatcher);

                ThreadPool.QueueUserWorkItem(new WaitCallback(
                    (object o) =>
                    {
                        try
                        {
                            service.Like(feed);
                            this.BeginInvoke(() =>
                                {
                                    if (feed.IsLiked)
                                        this.Notify("Buzz has been marked as liked");
                                    else
                                        this.Notify("Buzz has been marked as unliked");
                                });
                        }
                        catch (Exception ex)
                        {
                            this.Dispatcher.BeginInvoke(new Action(() => this.GBuzzException("WOops! error occurred while requesting on Google Buzz ...", ex)));
                        }
                    }));
            }
        }

        private void CloseProfile_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.ProfileGrid.Hide(200);
            this.BeginInvoke(() => this.ProfileGrid.Visibility = Visibility.Collapsed, 250);
        }

        private void GBuzzSearchButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string searchWord;
            var account = GetAccount();
            if (!string.IsNullOrWhiteSpace(searchWord = this.BuzzSearchTextBox.Text) && account != null)
            {
                string queryUrl = "https://www.googleapis.com/buzz/v1/activities/search?q=" + HttpUtility.UrlEncode(searchWord);
                var channel = new Channel { Title = searchWord + " / " + account.FullName, DownloadUrl = queryUrl, ChannelId = queryUrl };
                var existingChannel = account.Channels.FirstOrDefault(c => c.ChannelId == channel.ChannelId);
                if (existingChannel != null)
                    account.Channels.Remove(existingChannel);
                account.Channels.Add(channel);
                SocialService.GetSocialService(account).AsyncUpdate(channel);
                this.BuzzSearchTextBox.Text = string.Empty;
            }
        }

        private void AccountIsDefault_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            var checkBox = e.OriginalSource as CheckBox;
            var account = Util.DataContextAs<Account>(e.OriginalSource);
            if (checkBox != null && checkBox.IsChecked == true && account != null)
            {
                DataStore.GBuzzAccounts
                         .Where(a => a != account)
                         .ForEach(a => a.IsDefault = false);
            }
        }

        private Account GetAccount(RoutedEventArgs e)
        {
            var feed = Util.DataContextAs<Feed>(e.OriginalSource);
            if (feed != null) return GetAccount(feed);

            return DataStore.GBuzzAccounts.FirstOrDefault();
        }

        private Account GetAccount()
        {
            return DataStore.GBuzzAccounts.FirstOrDefault(a => a.IsDefault) ?? DataStore.GBuzzAccounts.FirstOrDefault();
        }

        private Account GetAccount(Feed feed)
        {
            return (from a in DataStore.GBuzzAccounts
                    from c in a.Channels
                    where c.Feeds.Contains(feed)
                    select a).FirstOrDefault();
        }

        private void PostComment_Click(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;
            if (button != null)
            {
                var commentControl = button.FindName("commentControl") as Border;
                var commentBox = button.FindName("commentBox") as TextBox;
                if (commentControl != null && commentBox != null && !string.IsNullOrEmpty(commentBox.Text))
                {
                    Account account;
                    var feed = Util.DataContextAs<Feed>(e.OriginalSource);
                    if (feed != null && (account = this.GetAccount(feed)) != null)
                    {
                        string comments = commentBox.Text;
                        //this.progressBar.IsRunning = true;
                        ThreadPool.QueueUserWorkItem(new WaitCallback(
                            (object o) =>
                            {
                                try
                                {
                                    var socialService = SocialService.GetSocialService(account);
                                    var commentFeed = socialService.Reply(feed, comments);

                                    if (commentFeed != null)
                                    {
                                        this.Dispatcher.Invoke(new Action(
                                            () =>
                                            {
                                                var person = commentFeed.People.First();
                                                feed.Comments.Add(new Comment
                                                {
                                                    Guid = commentFeed.Guid,
                                                    Content = Model.Content.ToPainText(commentFeed.TextDescription),
                                                    Person = new Person { Name = person.Name, UserId = person.UserId, ImageUrl = person.ImageUrl, Url = person.Url, Email = person.Email, Role = person.Role },
                                                    Published = commentFeed.Updated
                                                });
                                                this.Notify("Comments has been posted!");
                                            }));
                                    }

                                    this.Dispatcher.Invoke(new Action(() =>
                                    {
                                        commentBox.Clear();
                                        commentControl.Visibility = Visibility.Collapsed;
                                    }));

                                }
                                catch (Exception ex)
                                {
                                    this.Dispatcher.Invoke(new Action(() => this.LogAndShowError("WOops! error occurred while sending comments ...", ex)));
                                }
                            }));
                    }
                }
            }
        }

        private void CancelComment_Click(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;
            if (button != null)
            {
                var commentControl = button.FindName("commentControl") as Border;
                if (commentControl != null)
                    commentControl.Visibility = System.Windows.Visibility.Collapsed;
            }
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