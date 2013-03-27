using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Feedbook.Automation
{
    internal class DateTimeToOutlookStyleGroupConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is DateTime)
            {
                DateTime dateTime = ((DateTime)value).Date;
                DateTime dtNow = DateTime.Now.Date;

                if (dateTime == dtNow)
                    return "Today";
                    
                dtNow = dtNow.Subtract(new TimeSpan((int)dtNow.DayOfWeek,0,0,0));
                if (dateTime >= dtNow )
                    return dateTime.DayOfWeek.ToString();

                dtNow = dtNow.AddDays(-7);
                if (dateTime >= dtNow)
                    return "Last Week";

                dtNow = dtNow.Subtract(new TimeSpan(dtNow.Day, 0, 0, 0));

                if (dateTime >= dtNow)
                    return "Last Month";

                return "Old";
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
