using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace Feedbook.Automation
{
    internal class NullOrEmptyToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //Reverse effect
            if (parameter != null)
            {
                string paramStr = parameter.ToString().ToLower();
                if (paramStr == "true")
                    return (value == null || "".Equals(value)) ? Visibility.Visible : Visibility.Collapsed;
            }

            if (value == null)
                return Visibility.Collapsed;

            if (value is string && (string)value == string.Empty)
                return Visibility.Collapsed;

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
