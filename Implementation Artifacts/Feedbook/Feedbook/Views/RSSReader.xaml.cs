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
using System.Collections.ObjectModel;
using Feedbook.Model;
using Feedbook.Helper;
using System.Windows.Controls.Primitives;
using Feedbook.Services;
using System.Diagnostics;
using System.Windows.Media.Animation;
using System.Collections.Specialized;

namespace Feedbook.Views
{
    internal partial class RSSReader
    {
        private ObservableCollection<CategoryFeed> categoryFeeds = new ObservableCollection<CategoryFeed>();

        public RSSReader()
        {
            this.InitializeComponent();
            this.Loaded += (sender, e) =>
             {
                 this.RefreshFeeds();
                 this.BeginInvoke(() => this.SourcesRadioButton.IsChecked = true, 200);
                 GlobalEventManager.OnFeedAdd += this.OnFeedAdd;
                 DataStore.Channels.CollectionChanged += this.OnChannelsCollectionChanged;
                 this.OnChannelsCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
             };

            this.Unloaded += (sender, e) =>
                {
                    GlobalEventManager.OnFeedAdd -= this.OnFeedAdd;
                    DataStore.Channels.CollectionChanged -= this.OnChannelsCollectionChanged;
                };
        }

        private void OnChannelsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (DataStore.Channels.Count > 0)
            {
                this.RSSGrid.Visibility = Visibility.Visible;
                this.DiscoverRSSMessage.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.RSSGrid.Visibility = Visibility.Collapsed;
                this.DiscoverRSSMessage.Visibility = Visibility.Visible;
            }
        }

        private void RadioButton_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            var radioButton = e.OriginalSource as RadioButton;
            if (radioButton != null)
            {
                this.ChannelListBox.HideSlow(200);
                this.BeginInvoke(() =>
                {
                    this.ChannelListBox.ShowSlow(300);
                    switch (radioButton.Tag + string.Empty)
                    {
                        case "Sources":
                            {
                                this.ChannelListBox.ItemTemplate = ((DataTemplate)this.Resources["ChannelTemplate"]);
                                this.ChannelListBox.ItemsSource = ((CollectionViewSource)this.Resources["ChannelSource"]).View;
                                this.ChannelListBox.SelectedItem = DataStore.Channels.OrderBy(c => c.Title.ToPlainText).FirstOrDefault();
                            }
                            break;
                        case "Favorites":
                            {
                                this.ChannelListBox.ItemTemplate = ((DataTemplate)this.Resources["FavoriteFeedTemplate"]);
                                var viewSource = (CollectionViewSource)this.Resources["CategorySource"];
                                viewSource.Source = DataStore.FavoriteCategories;
                                this.ChannelListBox.ItemsSource = viewSource.View;
                                this.ChannelListBox.SelectedItem = DataStore.FavoriteCategories.OrderBy(c => c.Category).FirstOrDefault();
                            }
                            break;
                        case "Categories":
                            {
                                this.ChannelListBox.ItemTemplate = ((DataTemplate)this.Resources["CategoryFeedTemplate"]);
                                var viewSource = (CollectionViewSource)this.Resources["CategorySource"];
                                viewSource.Source = this.categoryFeeds;
                                this.ChannelListBox.ItemsSource = viewSource.View;
                                this.ChannelListBox.SelectedItem = this.categoryFeeds.OrderBy(c => c.Category).FirstOrDefault();
                            }
                            break;
                    }
                }, 250);
            }
        }

        private void OnFeedAdd(object sender, FeedEventArgs e)
        {
            if (e.Source == FeedSource.RSS)
            {
                foreach (var category in e.Feed.Categories)
                {
                    var categoryFeed = DataStore.FavoriteCategories.FirstOrDefault(c => string.Equals(category.Name, c.Category, StringComparison.OrdinalIgnoreCase));
                    if (categoryFeed != null)
                        categoryFeed.Feeds.Add(e.Feed);

                    categoryFeed = this.categoryFeeds.FirstOrDefault(c => string.Equals(category.Name, c.Category, StringComparison.OrdinalIgnoreCase));
                    if (categoryFeed == null)
                    {
                        categoryFeed = new CategoryFeed { Category = category.Name };
                        this.categoryFeeds.Add(categoryFeed);
                    }

                    categoryFeed.Feeds.Add(e.Feed);
                }
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var categoryFeed = Util.DataContextAs<CategoryFeed>(e.OriginalSource);
            if (categoryFeed != null)
                if (!DataStore.FavoriteCategories.Any(c => string.Equals(c.Category, categoryFeed.Category, StringComparison.OrdinalIgnoreCase)))
                    DataStore.FavoriteCategories.Add(categoryFeed);

            var button = e.OriginalSource as DependencyObject;
            if (button != null)
            {
                var popup = button.GetVisualAncestors()
                                  .OfType<Popup>()
                                  .FirstOrDefault();

                if (popup != null)
                    popup.Hide(Constants.Default.HideMilliseconds);
            }
        }

        private void RemoveButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var categoryFeed = Util.DataContextAs<CategoryFeed>(e.OriginalSource);
            if (categoryFeed != null)
                DataStore.FavoriteCategories.Remove(categoryFeed);

            if (this.ChannelListBox.SelectedItem == null)
                this.ChannelListBox.SelectedItem = DataStore.FavoriteCategories.FirstOrDefault();

            var button = e.OriginalSource as DependencyObject;
            if (button != null)
            {
                var popup = button.GetVisualAncestors()
                                  .OfType<Popup>()
                                  .FirstOrDefault();
                if (popup != null)
                    popup.Hide(Constants.Default.HideMilliseconds);
            }
        }

        private void DiscoveryRSS_Click(object sender, RoutedEventArgs e)
        {
            (new RSSDiscovery() { Owner = Window.GetWindow(this) }).ShowDialog();
            this.RefreshFeeds();
            if (this.ChannelListBox.SelectedItem == null)
                this.ChannelListBox.SelectedItem = DataStore.Channels.OrderBy(c => c.Title.ToPlainText).FirstOrDefault();
        }

        private void RefreshFeeds()
        {
            this.categoryFeeds.Clear();

            var feeds = (from channel in DataStore.Channels
                         from feed in channel.Feeds
                         from category in feed.Categories
                         select new { Category = category, Feed = feed }).Distinct();

            foreach (var feed in feeds)
            {
                var categoryFeed = this.categoryFeeds.FirstOrDefault(c => string.Equals(feed.Category.Name, c.Category, StringComparison.OrdinalIgnoreCase));
                if (categoryFeed == null)
                {
                    categoryFeed = new CategoryFeed { Category = feed.Category.Name };
                    this.categoryFeeds.Add(categoryFeed);
                }

                categoryFeed.Feeds.Add(feed.Feed);
            }
        }

        private void ChannelListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var feeds = this.GetFeedList();

            if (feeds != null)
                this.FeedListBox.SelectedItem = feeds.FirstOrDefault();
        }

        private IEnumerable<Feed> GetFeedList()
        {
            Channel channel;
            CategoryFeed categoryFeed;

            if ((channel = this.ChannelListBox.SelectedItem as Channel) != null)
                return channel.Feeds.OrderByDescending(f => f.Updated);

            if ((categoryFeed = this.ChannelListBox.SelectedItem as CategoryFeed) != null)
                return categoryFeed.Feeds.OrderByDescending(f => f.Updated);

            return null;
        }

        private void ChannelSettingButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var channel = Util.DataContextAs<Channel>(e.OriginalSource);
            if (channel != null)
                (new RSSChannelSettings { Channel = channel, Owner = Window.GetWindow(this) }).ShowDialog();
        }

        private void FeedView_Click(object sender, RoutedEventArgs e)
        {
            var enclosure = Util.DataContextAs<Enclosure>(e.OriginalSource);
            if (enclosure != null)
            {
                if (enclosure.IsMedia && enclosure.IsDownloaded)
                {
                    try
                    {
                        this.MediaEnclosureView.DataContext = enclosure;
                        this.ShowMediaView();

                        return;
                    }
                    catch (Exception ex)
                    {
                        this.LogAndShowError("WOops! error occurred while playing media", ex);
                    }
                }

                Util.OpenInBrowser(enclosure.Url);
            }
        }

        private void MediaButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.MediaEnclosureView.IsVisible())
                this.ShowFeedView();
            else
                this.ShowMediaView();
        }

        private void FeedListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                var feed = e.AddedItems[0] as Feed;
                if (feed != null)
                {
                    feed.IsReaded = true;
                    this.ShowFeedView();
                }
            }
        }

        private void ShowFeedView()
        {
            if (this.MediaEnclosureView.IsVisible())
            {
                this.MediaEnclosureView.HideSlow(200);
                this.BeginInvoke(() => this.FeedView.ShowSlow(300), 250);
                this.MainViewImage.Source = new BitmapImage(new Uri("pack://application:,,,/Feedbook;component/Images/podcast.png", UriKind.Absolute));
            }
        }

        private void ShowMediaView()
        {
            if (this.FeedView.IsVisible())
            {
                this.FeedView.HideSlow(200);
                this.BeginInvoke(() => this.MediaEnclosureView.ShowSlow(300), 250);
                this.MainViewImage.Source = new BitmapImage(new Uri("pack://application:,,,/Feedbook;component/Images/feed_cloud.png", UriKind.Absolute));
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var view = this.FeedListBox.ItemsSource as System.ComponentModel.ICollectionView;
            if (view != null)
            {
                var filter = (this.SeachTextBox.Text + "").ToLower();
                view.Filter = delegate(object item)
                {
                    var feed = item as Feed;
                    
                    if (string.IsNullOrWhiteSpace(filter))
                        return true;

                    return feed != null
                        && ((feed.Title != null && feed.Title.ToLower().Contains(filter))
                            || feed.TextDescription != null && feed.TextDescription.ToLower().Contains(filter)
                            || feed.Author != null && feed.Author.Name != null && feed.Author.Name.ToLower().Contains(filter)
                            || feed.Categories.Any(c => c.Name != null && c.Name.ToLower().Contains(filter)));
                };
                view.Refresh();
                if (this.FeedListBox.SelectedItem == null && this.FeedListBox.HasItems)
                    this.FeedListBox.SelectedIndex = 0;
            }
        }
    }
}