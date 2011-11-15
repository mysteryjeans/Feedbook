using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Feedbook.Automation
{
    internal class BytesToReadableSizeConverter : IValueConverter
    {
        private static readonly string[] Suffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                int suffixIndex = 0;
                try
                {
                    double size = System.Convert.ToDouble(value);

                    while (size >= 1024 && suffixIndex + 1 < Suffixes.Length)
                    {
                        suffixIndex++;
                        size /= 1024;
                    }

                    return String.Format("{0:0.##} {1}", size, Suffixes[suffixIndex]);
                }
                catch { }
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
