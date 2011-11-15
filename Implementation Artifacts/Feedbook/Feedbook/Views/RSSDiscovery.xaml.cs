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
using Feedbook.Custom;
using System.ComponentModel;
using Feedbook.Model;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using Feedbook.Helper;
using System.Net;
using System.Xml.Linq;
using CoreSystem.ValueTypeExtension;
using Feedbook.Services.WebFeed;
using System.Threading;
using CoreSystem.RefTypeExtension;

namespace Feedbook.Views
{
    /// <summary>
    /// Interaction logic for DiscoveryRSS.xaml
    /// </summary>
    internal partial class RSSDiscovery : FbWindow
    {
        internal class Subscribe : INotifyPropertyChanged, IComparable
        {
            private bool isSubscribed;

            public event PropertyChangedEventHandler PropertyChanged;

            public Channel Channel { get; set; }

            public bool IsSubscribed
            {
                get { return this.isSubscribed; }
                set
                {
                    if (this.isSubscribed != value)
                    {
                        this.isSubscribed = value;
                        if (this.PropertyChanged != null)
                            this.PropertyChanged(this, new PropertyChangedEventArgs("IsSubscribed"));
                    }
                }
            }

            #region IComparable Members

            public int CompareTo(object obj)
            {
                return this == obj ? 0 : 1;
            }

            #endregion
        }

        private static readonly Regex LinkRegex = new Regex(Constants.ANCHOR_LINK_TAG_REGEX, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex HrefRegex = new Regex(Constants.HREF_ATTRIBUTE_REGEX, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex TypeRegex = new Regex(Constants.TYPE_ATTRIBUTE_REGEX, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private Queue<string> urls = new Queue<string>();
        private ObservableCollection<Subscribe> channels = new ObservableCollection<Subscribe>();

        public RSSDiscovery()
        {
            InitializeComponent();
            this.ChannelListBox.ItemsSource = this.channels;
            if (this.UrlTextBox.Text != null)
                this.UrlTextBox.SelectionStart = this.UrlTextBox.Text.Length;
            this.Loaded += (sender, e) =>
            {
                this.UrlTextBox.UpdateLayout();
                this.UrlTextBox.Focus();
            };
        }       

        private void GoButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.UrlTextBox.Text != null && this.UrlTextBox.Text.Trim().Length > 0)
            {
                string url = this.UrlTextBox.Text.Trim();

                //Checking for valid url
                if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    this.urls.Clear();
                    this.urls.Enqueue(url);
                    this.channels.Clear();
                    this.DiscoverUrl();
                }
                else
                    FBMessageBox.Show("Url is not valid!");
            }
        }

        private void DiscoverUrl()
        {
            if (this.urls.Count > 0)
            {
                this.ProgressBar.IsIndeterminate = true;
                Downloader.DownloadStringAsync(urls.Dequeue(), this.DownloadProgressChanged, this.DownloadCompleted);
            }
        }

        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (e.TotalBytesToReceive != -1)
                this.ProgressBar.Value = e.ProgressPercentage;
            else if (!this.ProgressBar.IsIndeterminate)
                this.ProgressBar.IsIndeterminate = true;
        }

        private void DownloadCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (this.ProgressBar.IsIndeterminate)
                this.ProgressBar.IsIndeterminate = false;
            else
                this.ProgressBar.Value = 0;

            if (e.Cancelled)
            {
                FBMessageBox.Show("Download cancelled!", Constants.Caption.INFO, MessageBoxButton.OK);
            }
            else if (e.Error != null)
            {
                FBMessageBox.Show(e.Error.Message);
            }
            else
            {
                var url = e.UserState as string;
                DiscoverContent(e.Result, url);
            }
            this.Cursor = Cursors.Arrow;
        }

        private void DiscoverContent(string content, string downloadUrl)
        {
            try
            {
                XElement element = null;
                try { element = XElement.Parse(content, LoadOptions.PreserveWhitespace); }
                catch { }

                if (element == null || string.Compare(element.Name.LocalName, "html", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    foreach (Match linkMatch in LinkRegex.Matches(content))
                    {
                        Group urlGroup;
                        Group typeGroup;
                        var hrefMatch = HrefRegex.Match(linkMatch.Value);
                        var typeMatch = TypeRegex.Match(linkMatch.Value);

                        if (hrefMatch.Success && typeMatch.Success
                            && (urlGroup = hrefMatch.Groups["url"]) != null
                            && (typeGroup = typeMatch.Groups["type"]) != null
                            && Uri.IsWellFormedUriString(urlGroup.Value, UriKind.Absolute)
                            && typeGroup.Value.In("application/rss+xml", "application/atom+xml")
                            && !urls.Contains(urlGroup.Value))
                            urls.Enqueue(urlGroup.Value);
                    }

                    if (urls.Count == 0)
                        FBMessageBox.Show("No feed links found in this page!");
                }
                else
                {                    
                    this.Cursor = Cursors.Wait;
                    this.ProgressBar.IsIndeterminate = true;

                    ThreadPool.QueueUserWorkItem(new WaitCallback(
                        (object o) =>
                        {
                            try
                            {
                                int index;
                                if (content != null && (index = content.IndexOf('<')) != -1 && index != 0)
                                    content = content.Substring(index);

                                var subscribe = new Subscribe { Channel = (new FeedProcessor()).Parse(content, downloadUrl) };

                                if (!this.channels.Any(s => s.Channel.ChannelId == subscribe.Channel.ChannelId))
                                {
                                    subscribe.IsSubscribed = DataStore.Channels.Any(c => c.ChannelId == subscribe.Channel.ChannelId);
                                    this.Dispatcher.BeginInvoke(new Action(() => this.channels.Add(subscribe)));
                                }
                            }
                            catch (Exception ex)
                            {
                                this.BeginInvoke(() => this.LogAndShowError("WOops! error occurred while parsing feed channel", 
                                                                            ex.AddData("RSS Url", downloadUrl)
                                                                              .AddData("Content", content)));
                            }
                            finally
                            {
                                this.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    this.ProgressBar.IsIndeterminate = false;
                                    this.ProgressBar.Value = 0;
                                    this.Cursor = Cursors.Arrow;
                                }));
                            }
                        }));
                }
            }
            catch (Exception ex)
            {
                this.LogAndShowError("WOops! error occurred while discovering feed channels", ex);
            }

            DiscoverUrl();
        }

        private void SubscribeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Subscribe subcribe = Util.DataContextAs<Subscribe>(e.OriginalSource);
                if (subcribe != null)
                {
                    if (!DataStore.Channels.Any(c => string.Equals(c.ChannelId, subcribe.Channel.ChannelId, StringComparison.OrdinalIgnoreCase)))
                        DataStore.Channels.Add(subcribe.Channel);

                    subcribe.IsSubscribed = true;
                }
            }
            catch (Exception ex)
            {
                this.LogAndShowError("WOops! something goes wrong while subscribing to this channel", ex);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
