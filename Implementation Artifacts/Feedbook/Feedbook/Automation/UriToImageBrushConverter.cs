using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Feedbook.Helper;

namespace Feedbook.Automation
{
    internal class UriToImageBrushConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var valueStr = value as string;
            if (!string.IsNullOrEmpty(valueStr))
            {
                try
                {
                    Uri imageSource;
                    if (valueStr.StartsWith("http://"))
                        imageSource = ImageCache.GetCacheImageUri(valueStr);
                    else
                        imageSource = new Uri(valueStr, UriKind.RelativeOrAbsolute);

                    return new ImageBrush { ImageSource = new BitmapImage(imageSource) };
                }
                catch (Exception ex)
                {
                    ex.Data["ImageUrl"] = value;
                    this.Log("Error occurred converting image url to image brush", ex);
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
