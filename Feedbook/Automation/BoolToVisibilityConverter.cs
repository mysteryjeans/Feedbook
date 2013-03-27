﻿using System;
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

namespace Feedbook.Automation
{
    internal class BoolToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Checking for invert 
            if (parameter != null && string.Equals(parameter.ToString(), "True", StringComparison.OrdinalIgnoreCase))
            {
                if (value is bool && (bool)value)
                    return Visibility.Collapsed;

                return Visibility.Visible;
            }
            else
            {
                if (value is bool && (bool)value)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
