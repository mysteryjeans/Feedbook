using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows;

namespace Feedbook.Helper
{
    internal static class GeneralExtension
    {
        public static void Log(this object obj, string message, Exception exception)
        {
            ILog logger = LogManager.GetLogger(obj.GetType());
            logger.Error(message, exception);
        }

        public static void WebRequestException(this object obj, string message, Exception exception)
        {
            var webException = exception as WebException;
            if (webException == null || webException.Response != null || NetworkInterface.GetIsNetworkAvailable())
                obj.LogAndNotify(message, exception);
        }


        public static void LogAndShow(this object obj, string message, Exception exception)
        {
            obj.Log(message, exception);
            FBMessageBox.ShowError(message, exception);
        }

        public static void LogAndNotify(this object obj, string message, Exception exception)
        {
            obj.Log(message, exception);

            MainWindow mainWindow;
            if (Application.Current != null && (mainWindow = Application.Current.MainWindow as MainWindow) != null)
                mainWindow.Notify(message);
        }
    }
}
