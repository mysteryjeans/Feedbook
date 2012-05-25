using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Feedbook.Model;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using Feedbook.Helper;

namespace Feedbook.Automation
{
    internal class EnclosureToBitmapImageConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var enclosure = value as Enclosure;
            if (enclosure != null)
            {
                var previewLink = enclosure.Links.FirstOrDefault(l => l.LinkRel == LinkRel.Preview);
                string imageUrl = previewLink != null ? previewLink.HRef : enclosure.Url;
                try
                {
                    return new BitmapImage(imageUrl.StartsWith("http://")
                                           ? ImageCache.GetCacheImageUri(imageUrl)
                                           : new Uri(imageUrl, UriKind.RelativeOrAbsolute));
                }
                catch (Exception ex)
                {
                    ex.Data["EnclosureXML"] = Util.Serialize(enclosure);
                    ex.Data["imageUrl"] = imageUrl;
                    this.Log("Cannot convert enclosure to image preview", ex);
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
