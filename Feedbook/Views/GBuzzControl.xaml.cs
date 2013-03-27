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
using Feedbook.Model;
using Feedbook.Services.Google.Picasa;
using System.Collections.ObjectModel;
using Feedbook.Services;
using System.Text.RegularExpressions;
using Feedbook.Services.Google.Buzz;
using Feedbook.Services.Security;
using CoreSystem.ValueTypeExtension;
using Microsoft.Win32;
using System.IO;

namespace Feedbook.Views
{
    /// <summary>
    /// Interaction logic for TweetControl.xaml
    /// </summary>
    internal partial class GBuzzControl : UserControl
    {

        private static readonly Regex ImageRegex = new Regex(Constants.IMAGE_TAG_REGEX, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex SrcRegex = new Regex(Constants.SRC_ATTRIBUTE_REGEX, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex TypeRegex = new Regex(Constants.TYPE_ATTRIBUTE_REGEX, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private string albumId;
        private List<string> queriedUri = new List<string>();
        private ObservableCollection<Attachment> attachments = new ObservableCollection<Attachment>();
        private ObservableCollection<Attachment> discoveredImages = new ObservableCollection<Attachment>();

        public string Buzz
        {
            set
            {
                this.BuzzBox.Text = value ?? string.Empty;
                this.BuzzBox.SelectionStart = this.BuzzBox.Text.Length;
                this.BuzzBox.Focus();
            }
        }

        public GBuzzControl()
        {
            InitializeComponent();
            this.AttachmentListBox.ItemsSource = this.attachments;
            this.ImageAttachmentListBox.ItemsSource = this.discoveredImages;
        }

        private void SendButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string post = this.BuzzBox.Text;
            var accounts = this.GetSelectedAccounts();

            if (accounts.Length > 0 && !string.IsNullOrWhiteSpace(post))
            {
                //this.progressBar.IsIndeterminate = true;
                ThreadPool.QueueUserWorkItem(new WaitCallback(
                    (object o) =>
                    {
                        try
                        {
                            foreach (var account in accounts)
                            {
                                var service = new GBuzzSocialService(account);
                                service.Post(post, this.attachments.ToArray());
                            }

                            this.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                this.attachments.Clear();
                                this.discoveredImages.Clear();
                                this.BuzzBox.Text = string.Empty;
                                this.BuzzBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                                //this.Notify("Activity has been posted!");
                            }));
                        }
                        catch (Exception ex)
                        {
                            this.BeginInvoke(() => this.LogAndShowError("WOops! error occurred while posting on Google Buzz ...", ex));
                        }
                    }));
            }
            else
            {
                this.queriedUri.Clear();
                this.attachments.Clear();
                this.discoveredImages.Clear();
            }
        }

        private void BuzzBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (this.BuzzBox.SelectionStart > 0)
            {
                string text = this.BuzzBox.Text.Substring(0, this.BuzzBox.SelectionStart);
                Uri lastUri = Util.GetLastUri(text);
                if (lastUri != null)
                {
                    string uriString = lastUri.ToString().Trim();
                    if (!queriedUri.Contains(uriString))
                    {
                        queriedUri.Add(uriString);

                        this.StatusTextBlock.Text = "Searching ...";
                        ThreadPool.QueueUserWorkItem(new WaitCallback(
                            (object o) =>
                            {
                                string url = (string)o;

                                try
                                {
                                    if (url.StartsWith("http://www.youtube.com/watch?") || url.StartsWith("http://youtube.com/watch?"))
                                    {
                                        try
                                        {
                                            var queryParameters = OAuthHelper.ParseQueryString(url.Replace("http://www.youtube.com/watch?", "").Replace("http://youtube.com/watch?", ""));
                                            string videoId = queryParameters["v"];
                                            if (!string.IsNullOrEmpty(videoId))
                                            {
                                                var attachment = new Attachment
                                                {
                                                    Title = videoId,
                                                    SocialType = SocialType.Video
                                                };

                                                attachment.Links.Add(new Link
                                                {
                                                    HRef = "http://www.youtube.com/v/" + videoId,
                                                    Rel = "alternate",
                                                    Type = "application/x-shockwave-flash",
                                                    Title = videoId
                                                });

                                                attachment.Links.Add(new Link
                                                {
                                                    HRef = string.Format("http://img.youtube.com/vi/{0}/2.jpg", videoId),
                                                    Rel = "preview",
                                                    Type = "image/jpeg",
                                                    Title = videoId
                                                });

                                                attachment.Links.Add(new Link
                                                {
                                                    HRef = "http://www.youtube.com/watch?" + url.Replace("http://www.youtube.com/watch?", "").Replace("http://youtube.com/watch?", ""),
                                                    Rel = "alternate",
                                                    Type = "text/html"
                                                });

                                                this.Dispatcher.BeginInvoke(new Action(() => this.attachments.Add(attachment)));
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            this.BeginInvoke(() => this.LogAndShowError("WOops! error occurred while looking for video attachments at entered uri", ex));
                                        }
                                    }
                                    else
                                    {
                                        var content = Downloader.DownloadString(url);
                                        foreach (Match linkMatch in ImageRegex.Matches(content))
                                        {
                                            string imageType;
                                            Group srcGroup;
                                            Group typeGroup;
                                            var srcMatch = SrcRegex.Match(linkMatch.Value);
                                            var typeMatch = TypeRegex.Match(linkMatch.Value);

                                            if (srcMatch.Success
                                                && (srcGroup = srcMatch.Groups["src"]) != null
                                                && Uri.IsWellFormedUriString(srcGroup.Value, UriKind.Absolute)
                                                && ((typeMatch.Success
                                                    && (typeGroup = typeMatch.Groups["type"]) != null
                                                    && (imageType = typeGroup.Value) != null
                                                    && imageType.In("image/png", "image/jpeg", "image/gif", "image/bmp"))
                                                    || (imageType = GetImageType(srcGroup.Value)).In("image/png", "image/jpeg", "image/gif", "image/bmp")))
                                            {
                                                var count = (from attachment in this.discoveredImages
                                                             from link in attachment.Links
                                                             where link.HRef == srcGroup.Value
                                                             select link).Count();


                                                if (count == 0)
                                                {
                                                    string fileName = System.IO.Path.GetFileName((new Uri(srcGroup.Value)).LocalPath);
                                                    var attachment = new Attachment
                                                    {
                                                        Title = fileName,
                                                        SocialType = SocialType.Photo
                                                    };

                                                    attachment.Links.Add(new Link
                                                    {
                                                        HRef = srcGroup.Value,
                                                        Rel = "enclosure",
                                                        Type = imageType,
                                                        Title = fileName
                                                    });

                                                    attachment.Links.Add(new Link
                                                    {
                                                        HRef = url,
                                                        Rel = "alternate",
                                                        Type = "text/html"
                                                    });


                                                    this.Dispatcher.BeginInvoke(new Action(() => this.discoveredImages.Add(attachment)));
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    this.BeginInvoke(() => this.LogAndShowError("WOops! something goes wrong while loading photos from link", ex));
                                }
                                finally
                                {
                                    this.Dispatcher.BeginInvoke(new Action(() => this.StatusTextBlock.Text = string.Empty));
                                }

                            }), lastUri.OriginalString);
                    }
                }
            }
        }

        private void Link_Click(object sender, RoutedEventArgs e)
        {
            string url = Util.DataContextAs<string>(e.OriginalSource);
            if (url != null)
                Util.OpenInBrowser(url);
        }

        private void Attachment_Cross_Click(object sender, RoutedEventArgs arg)
        {
            var attachment = Util.DataContextAs<Attachment>(arg.OriginalSource);
            if (attachment != null)
                this.attachments.Remove(attachment);
        }

        private void Add_Image_Click(object sender, RoutedEventArgs arg)
        {
            var attachment = Util.DataContextAs<Attachment>(arg.OriginalSource);
            if (attachment != null)
            {
                this.attachments.Add(attachment);
                this.discoveredImages.Remove(attachment);
            }
        }

        private void AddPhoto_Click(object sender, RoutedEventArgs e)
        {
            var acct = DataStore.GBuzzAccounts.FirstOrDefault(a => a.IsDefault)
                       ?? this.GetSelectedAccounts().FirstOrDefault()
                       ?? DataStore.GBuzzAccounts.FirstOrDefault();

            if (acct == null)
            {
                FBMessageBox.Show("Please add an google buzz account first!");
                return;
            }

            var filedialog = new OpenFileDialog();
            filedialog.Multiselect = true;
            filedialog.Filter = "Images (*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG";

            if (filedialog.ShowDialog() == true)
            {
                this.StatusTextBlock.Text = "Attaching ...";
                ThreadPool.QueueUserWorkItem(new WaitCallback(
                (object o) =>
                {
                    object[] args = (object[])o;
                    Account account = (Account)args[0];
                    string[] imageFiles = (string[])args[1];

                    try
                    {
                        foreach (var imageFile in imageFiles)
                        {
                            string photoUrl;
                            string fileName = System.IO.Path.GetFileName(imageFile);
                            string fileExtension = System.IO.Path.GetExtension(imageFile).TrimStart('.');

                            if (string.IsNullOrEmpty(albumId))
                                this.albumId = GPicasaService.CreateAlbum(account.UserName, account.Token, account.TokenSecret);

                            PicasaImageFormat imageFormat = GetImageFormat(fileExtension);
                            using (var imageStream = File.OpenRead(imageFile))
                            {
                                photoUrl = GPicasaService.UploadPhoto(account.UserName, this.albumId, fileName, imageStream, imageFormat, account.Token, account.TokenSecret);
                                var attachment = new Attachment
                                {
                                    Title = fileName,
                                    SocialType = SocialType.Photo
                                };

                                attachment.Links.Add(new Link
                                {
                                    HRef = photoUrl,
                                    Rel = "enclosure",
                                    Type = "image/" + fileExtension,
                                    Title = fileName
                                });

                                this.BeginInvoke(() =>
                                    {
                                        this.attachments.Add(attachment);
                                        this.Notify("Photo(s) have been uploaded on your Picasa Web Album!");
                                    });

                            }
                        }
                    }
                    catch (NotSupportedException ex)
                    {
                        this.BeginInvoke(() => FBMessageBox.Show(ex.Message));
                    }
                    catch (Exception ex)
                    {
                        this.BeginInvoke(() =>
                          {
                              if (ex.HttpStatusCode() == HttpStatusCode.NotFound)
                              {
                                  FBMessageBox.Show(string.Format("Your Google account didn't haven't Picasa Web Albums product, you need to add this product to your Google account '{0}', before you can start uploading local photos", account.FullName));
                                  Util.OpenInBrowser("http://picasaweb.google.com/");
                              }
                              else
                                  this.GBuzzException("WOops! something goes wrong while uploading photo ...", ex);
                          });
                    }
                    finally
                    {
                        this.Dispatcher.BeginInvoke(new Action(() => this.StatusTextBlock.Text = string.Empty));
                    }
                }), new object[] { acct, filedialog.FileNames });
            }
        }

        private void CloseBuzzBoxButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.BuzzBox.Text = string.Empty;
            this.BuzzBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
            this.BuzzBox.Height = 25;

            this.CloseBuzzBoxButton.Visibility
             = this.BottonGrid.Visibility
             = Visibility.Collapsed;

            this.attachments.Clear();
            this.discoveredImages.Clear();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.AccountListBox.ItemsSource = DataStore.GBuzzAccounts;
            this.AccountListBox.SelectedItem = DataStore.GBuzzAccounts.FirstOrDefault(a => a.IsDefault) ?? DataStore.GBuzzAccounts.FirstOrDefault();
        }

        private void BuzzBox_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            this.BuzzBox.Height = 50;
            this.CloseBuzzBoxButton.Visibility
             = this.BottonGrid.Visibility
             = Visibility.Visible;
        }

        private void BuzzBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.BuzzBox.Text) && !this.IsMouseOver)
            {
                this.attachments.Clear();
                this.discoveredImages.Clear();

                this.BuzzBox.Height = 25;
                this.CloseBuzzBoxButton.Visibility
                 = this.BottonGrid.Visibility
                 = Visibility.Collapsed;
            }
        }

        private Account[] GetSelectedAccounts()
        {
            var accounts = new List<Account>();
            foreach (var selectedAccount in this.AccountListBox.SelectedItems)
            {
                if (selectedAccount as Account != null)
                    accounts.Add((Account)selectedAccount);
            }

            return accounts.ToArray();
        }

        private string GetImageType(string uri)
        {
            string extension = System.IO.Path.GetExtension((new Uri(uri)).LocalPath).TrimStart('.').ToLower();
            if (extension != null)
                return "image/" + (extension == "jpg" ? "jpeg" : extension);

            return "image";
        }

        private PicasaImageFormat GetImageFormat(string imageExtension)
        {
            switch (imageExtension.ToLower())
            {
                case "png":
                    return PicasaImageFormat.PNG;
                case "jpg":
                case "jpeg":
                    return PicasaImageFormat.JPEG;
                case "gif":
                    return PicasaImageFormat.JIF;
                case "bmp":
                    return PicasaImageFormat.BMP;
                default:
                    throw new NotSupportedException(string.Format("Picasa web album doesn't supported image format: '{0}'", imageExtension));
            }
        }
    }
}