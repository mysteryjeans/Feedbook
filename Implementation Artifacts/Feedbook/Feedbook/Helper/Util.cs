using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using CoreSystem.Util;
using System.Threading;
using System.IO;
using System.Windows.Threading;
using CoreSystem.ValueTypeExtension;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using Feedbook.Views;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using log4net;

namespace Feedbook.Helper
{
    internal static class Util
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Util));

        public static T DataContextAs<T>(object frameworkElement) where T : class
        {
            FrameworkElement element = frameworkElement as FrameworkElement;
            return element != null ? element.DataContext as T : null;
        }

        public static void BeginInvoke(Dispatcher dispatcher, Action action, TimeSpan wait)
        {
            BeginInvoke(new Action(() => dispatcher.BeginInvoke(action)), wait);
        }

        public static void BeginInvoke(Action action, TimeSpan wait)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback((o) =>
            {
                if (wait != TimeSpan.Zero)
                    Thread.Sleep(wait);

                action();
            }));
        }

        public static Dispatcher GetDispatcher()
        {
            return Application.Current.Dispatcher;
        }

        public static Uri GetLastUri(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                if (text[text.Length - 1].In(' ', ',', '\n', ';', '\t'))
                {
                    string[] words = text.Split(' ', ',', '\n', ';', '\t');
                    if (words != null && words.Length > 1)
                    {
                        string lastWord = words[words.Length - 2];
                        if (lastWord != null && lastWord.StartsWith("http") && Uri.IsWellFormedUriString(lastWord, UriKind.Absolute))
                            return new Uri(lastWord);
                    }
                }
            }

            return null;
        }

        public static void OpenInBrowser(string url)
        {
            try { Process.Start(url); }
            catch (Exception ex)
            {
                var message = string.Format("Failed to open url: '{0}'", url);
                Logger.Error(message, ex);
                FBMessageBox.ShowError(message, ex);
            }
        }

        internal static string ApplicationAssembly()
        {
            return Assembly.GetExecutingAssembly().GetName().ToString();
        }

        public static string Serialize(object objectToSerialize)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                var serializer = new DataContractSerializer(objectToSerialize.GetType());
                serializer.WriteObject(ms, objectToSerialize);
                ms.Position = 0;

                using (StreamReader reader = new StreamReader(ms))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static T Deserialize<T>(string jsonString)
        {
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {
                var serializer = new DataContractSerializer(typeof(T));
                return (T)serializer.ReadObject(ms);
            }
        }

        public static void SetProxy(this WebClient client)
        {
            if (client.Proxy != null && client.Proxy.Credentials == null)
                client.Proxy.Credentials = CredentialCache.DefaultNetworkCredentials;
        }

        public static void SetProxy(this WebRequest request)
        {
            if (request.Proxy != null && request.Proxy.Credentials == null)
                request.Proxy.Credentials = CredentialCache.DefaultNetworkCredentials;
        }
    }
}
