using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Feedbook.Helper;

namespace Feedbook.Automation
{
    internal class StringToUriConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strValue = value as string;

            if (strValue != null)
                try { return new Uri(strValue); }
                catch (Exception ex)
                {
                    ex.Data["string"] = strValue;
                    this.Log("Failed to convert string to Uri", ex);
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
