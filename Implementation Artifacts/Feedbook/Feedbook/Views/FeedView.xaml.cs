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
using Feedbook.Model;
using System.IO;
using System.Xml;
using System.Windows.Markup;
using System.Text.RegularExpressions;
using Feedbook.Helper;
using System.Windows.Media.Animation;
using System.Diagnostics;
using CoreSystem.RefTypeExtension;
using Feedbook.Mail;
using System.Web;
using System.Windows.Controls.Primitives;

namespace Feedbook
{
    /// <summary>
    /// Interaction logic for FeedView.xaml
    /// </summary>
    internal partial class FeedView : UserControl
    {
        private static readonly Regex ImageRegex = new Regex(Constants.IMAGE_TAG_REGEX, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex SrcRegex = new Regex(Constants.SRC_ATTRIBUTE_REGEX, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex TypeRegex = new Regex(Constants.TYPE_ATTRIBUTE_REGEX, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private Storyboard feedTransitionStory;

        public FeedView()
        {
            this.InitializeComponent();
            this.feedTransitionStory = (Storyboard)this.Resources["FeedTransitionStory"];
            this.Loaded += (sender, e) => this.feedTransitionStory.Begin();
        }

        private void UserControl_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            var feed = this.DataContext as Feed;
            if (feed != null)
            {
                try
                {
                    this.ContentGrid.Show();
                    this.feedTransitionStory.Begin();
                    
                    FlowDocument document;
                    try
                    {
                        string xamlString = HTMLConverter.HtmlToXamlConverter.ConvertHtmlToXaml(ReplaceImageUris(feed.EncodedDescription), true);
                        document = XamlReader.Load(new XmlTextReader(new StringReader(xamlString))) as FlowDocument;
                    }
                    catch (Exception ex)
                    {
                        this.Log("WOops! error encounter while rendering feed display", ex);
                        string xamlString = HTMLConverter.HtmlToXamlConverter.ConvertHtmlToXaml(ReplaceImageUris(feed.TextDescription), true);
                        document = XamlReader.Load(new XmlTextReader(new StringReader(xamlString))) as FlowDocument;
                    }

                    document.FontSize = 14;
                    document.FontFamily = new FontFamily("Calibri");
                    this.FeedReader.Document = document;

                    var hyperlinks = from paragraph in document.Blocks.OfType<Paragraph>()
                                     from hypelink in paragraph.Inlines.OfType<Hyperlink>()
                                     select hypelink;

                    foreach (var link in hyperlinks)
                        SetPreview(link);
                }
                catch (Exception ex)
                {
                    this.LogAndShow("WOops! error encounter while rendering feed display", ex);
                }
            }
            else
                this.ContentGrid.Hide();
        }

        private void SetPreview(Hyperlink link)
        {
            var previewImage = new Image { Margin = new Thickness(4) };

            previewImage.Loaded += (sender, e) =>
                {
                    try
                    {
                        //var imageURi = ThumbnailHelper.GetThumbnailImage(link.NavigateUri.OriginalString);
                        //var previewUri = new Uri(string.Format("http://api1.thumbalizr.com/?url={0}&width=250", HttpUtility.UrlEncode(link.NavigateUri.OriginalString)), UriKind.Absolute);
                        var previewUri = ImageCache.GetCacheImageUri("http://open.thumbshots.org/image.pxf?url=" + HttpUtility.UrlEncode(link.NavigateUri.OriginalString));
                        previewImage.Source = new BitmapImage(previewUri);
                    }
                    catch { }
                };

            link.ToolTip = previewImage;
        }

        private static string ReplaceImageUris(string stringContent)
        {
            foreach (Match linkMatch in ImageRegex.Matches(stringContent))
            {
                Group srcGroup;
                var srcMatch = SrcRegex.Match(linkMatch.Value);

                if (srcMatch.Success
                    && (srcGroup = srcMatch.Groups["src"]) != null
                    && Uri.IsWellFormedUriString(srcGroup.Value, UriKind.Absolute)
                    && srcGroup.Value != null && srcGroup.Value.StartsWith("http://"))
                {
                    stringContent = stringContent.Replace(srcGroup.Value, ImageCache.GetCacheImage(srcGroup.Value));
                }
            }

            return stringContent;
        }

        private void HyperLink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(e.Uri.ToString());
                e.Handled = true;
            }
            catch (Exception ex)
            {
                this.LogAndShowError("Failed to open link", ex.AddData("Url", e.Uri));
            }
        }

        private void SendEmailClick(object sender, System.Windows.RoutedEventArgs e)
        {
            var feed = this.DataContext as Feed;
            if (feed != null)
            {
                try
                {
                    var body = string.Format(@"
{0}

Source: {1}


Send from my Feedbook, social client for Windows ({2})


", feed.TextDescription, feed.Link.HRef, Constants.WEBSITE_URL);

                    MAPI mapi = new MAPI();
                    mapi.SendMailPopup(feed.Title, body);
                }
                catch (Exception ex)
                {
                    try { ex.Data["FeedXml"] = Util.Serialize(feed); }
                    catch { }
                    this.LogAndShowError("WOops! error occurred while preparing email", ex);
                }
            }
        }
    }
}