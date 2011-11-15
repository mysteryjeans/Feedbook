using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Feedbook.Automation
{
    internal class ContentTypeToImageUriConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string contentType = value as string;
            if (contentType != null)
            {
                if (contentType.StartsWith("audio"))
                    return Constants.Resources.IMAGE_AUDIO;

                if (contentType.StartsWith("video"))
                    return Constants.Resources.IMAGE_VIDEO;

                if (contentType.StartsWith("application"))
                    return Constants.Resources.IMAGE_APPLICATION;

                if(contentType.StartsWith("text"))
                    return Constants.Resources.IMAGE_ATTACHMENT;

                return Constants.Resources.IMAGE_ENCLOSURE;
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
