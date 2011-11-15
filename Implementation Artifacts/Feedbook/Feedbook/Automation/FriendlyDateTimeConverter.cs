using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Feedbook.Automation
{
    internal class FriendlyDateTimeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is DateTime)
            {
                var date = (DateTime)value;

                var timespan = DateTime.Now - date;

                if (timespan.Days > 365)
                    return string.Format("{0} year{1} ago", timespan.Days / 365, (timespan.Days / 365) > 1 ? "s" : "");

                if (timespan.Days > 30)
                    return string.Format("{0} month{1} ago", timespan.Days / 30, (timespan.Days / 30) > 1 ? "s" : "");
                
                if (timespan.Days > 0)
                    return string.Format("{0} day{1} ago", timespan.Days, timespan.Days > 1 ? "s" : "");

                if (timespan.Hours > 0)
                    return string.Format("{0} hour{1} ago", timespan.Hours, timespan.Hours > 1 ? "s" : "");
                
                if (timespan.Minutes > 0)
                    return string.Format("{0} minute{1} ago", timespan.Minutes, timespan.Minutes > 1 ? "s" : "");

                return "A moment ago";
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
