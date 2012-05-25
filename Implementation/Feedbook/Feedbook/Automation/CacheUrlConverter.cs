using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Feedbook.Helper;

namespace Feedbook.Automation
{
    internal class CacheUrlConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var valueStr = value as string;
            if (valueStr != null && valueStr.ToLower().StartsWith("http://"))
                try { return ImageCache.GetCacheImage(valueStr); }
                catch (Exception ex)
                {
                    ex.Data["Image Url"] = value;
                    this.Log("Failed to get cache image url", ex);
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
