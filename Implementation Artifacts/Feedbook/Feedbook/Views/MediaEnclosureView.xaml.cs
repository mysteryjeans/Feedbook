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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Feedbook.Model;
using CoreSystem.RefTypeExtension;
using Feedbook.Helper;
using System.Diagnostics;
using System.IO;

namespace Feedbook.Views
{
    /// <summary>
    /// Interaction logic for MediaEnclosureView.xaml
    /// </summary>
    internal partial class MediaEnclosureView : UserControl
    {
        private ObservableCollection<Enclosure> enclosures = new ObservableCollection<Enclosure>();

        public MediaEnclosureView()
        {
            InitializeComponent();
            this.LayoutGrid.Visibility = System.Windows.Visibility.Hidden;
            this.enclosures.CollectionChanged += (sender, e) =>
            {
                if (this.enclosures.Count == 0)
                {
                    this.LayoutGrid.Hide();
                    this.NoPodcastMessage.Show();
                }
                else
                {
                    this.LayoutGrid.Show();
                    this.NoPodcastMessage.Hide();
                }
            };

            enclosures.AddRange(from channel in DataStore.Channels
                                from feed in channel.Feeds
                                from enclosure in feed.Enclosures
                                where enclosure.IsMedia
                                orderby feed.Updated descending
                                select enclosure);

            this.Loaded += (sender, e) =>
            {
                GlobalEventManager.OnFeedAdd += new FeedEventHandler(OnFeedAdd);
                GlobalEventManager.OnFeedRemove += new FeedEventHandler(OnFeedRemove);
            };

            this.Unloaded += (sender, e) =>
            {
                GlobalEventManager.OnFeedAdd -= new FeedEventHandler(OnFeedAdd);
                GlobalEventManager.OnFeedRemove -= new FeedEventHandler(OnFeedRemove);
            };

            var enclosureSource = this.Resources["EnclosureSource"] as CollectionViewSource;
            if (enclosureSource != null)
                enclosureSource.Source = enclosures;


            this.EnclosureListBox.SelectedItem = null;
        }

        private void OnFeedAdd(object sender, FeedEventArgs e)
        {
            this.enclosures.AddRange(e.Feed.Enclosures.Where(en => en.IsMedia));
        }

        private void OnFeedRemove(object sender, FeedEventArgs e)
        {
            e.Feed.Enclosures
                  .Where(en => en.IsMedia)
                  .ForEach(en => this.enclosures.Remove(en));
        }

        private void Enclosure_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var enclosure = this.EnclosureListBox.SelectedItem as Enclosure;
            if (enclosure != null && enclosure.IsDownloaded)
                this.MediaPlayer.Play();
        }

        private void Enclosure_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var enclosure = Util.DataContextAs<Enclosure>(e.OriginalSource);
            if (enclosure != null)
            {
                try
                {
                    ContextMenu menu = new ContextMenu { Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint, DataContext = enclosure };
                    if (enclosure.IsDownloaded)
                    {
                        var play = new MenuItem { Header = "Play", FontWeight = FontWeights.Bold };
                        play.Click += (sender1, e1) =>
                        {
                            if (this.MediaPlayer.DataContext == enclosure)
                                this.MediaPlayer.Stop();
                            Process.Start(enclosure.LocalPath);
                        };

                        menu.Items.Add(play);

                        var openFolder = new MenuItem { Header = "Open Containing Folder" };
                        openFolder.Click += (sender1, e1) =>
                        {
                            ProcessStartInfo info = new ProcessStartInfo("explorer.exe", string.Format("/select,\"{0}\"", enclosure.LocalPath));
                            Process.Start(info);
                        };

                        menu.Items.Add(openFolder);

                        menu.Items.Add(new Separator());


                        var reDownload = new MenuItem { Header = "Re-Download" };
                        reDownload.Click += (sender2, e2) =>
                        {
                            try
                            {
                                if (File.Exists(enclosure.LocalPath))
                                    File.Delete(enclosure.LocalPath);

                                enclosure.IsDownloaded = false;
                                enclosure.DownloadedOn = null;
                            }
                            catch (Exception ex)
                            {
                                FBMessageBox.ShowError(ex);
                            }
                        };
                        menu.Items.Add(reDownload);


                    }
                    else
                    {
                        if (enclosure.IsDownloading
                            || enclosure.DownloadPodcast == true
                            || (enclosure.Channel != null && enclosure.Channel.DownloadPodcasts && enclosure.DownloadPodcast == null))
                        {
                            var excludeFromDownload = new MenuItem { Header = "Excluded", ToolTip = "Remove from download queue" };
                            excludeFromDownload.Click += (sender1, e1) => enclosure.DownloadPodcast = false;
                            menu.Items.Add(excludeFromDownload);
                        }
                        else
                        {
                            var excludeFromDownload = new MenuItem { Header = "Download", ToolTip = "Add to download queue" };
                            excludeFromDownload.Click += (sender1, e1) => enclosure.DownloadPodcast = false;
                            menu.Items.Add(excludeFromDownload);
                        }
                    }


                    var copyLink = new MenuItem { Header = "Copy Download Link" };
                    copyLink.Click += (sender3, e3) => Clipboard.SetText(enclosure.Url);
                    menu.Items.Add(copyLink);

                    menu.IsOpen = true;
                }
                catch (Exception ex)
                {
                    this.LogAndShowError("WOops! error occurred while showing context menu", ex);
                }
            }
        }

        private void EnclosureListBox_PreviewMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
                e.Handled = true;
            else
            {
                var enclosure = Util.DataContextAs<Enclosure>(e.OriginalSource);
                if (enclosure != null)
                    e.Handled |= !enclosure.IsDownloaded;
            }
        }
    }
}
