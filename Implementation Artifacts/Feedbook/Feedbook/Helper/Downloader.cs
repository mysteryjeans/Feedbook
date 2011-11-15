/*/
 *
 * Author: Faraz Masood Khan
 * 
 * Date: Wednesday, July 31, 2008 6:35 PM
 * 
 * Class: Downloader
 * 
 * Email: mk.faraz@gmail.com
 * 
 * Blogs: http://farazmasoodkhan.wordpress.com
 *
 * Website: http://www.linkedin.com/in/farazmasoodkhan
 *
 * Copyright: Faraz Masood Khan @ Copyright ©  2008
 *
/*/

using System;
using System.IO;
using System.Net;

using Feedbook.Helper;

namespace Feedbook.Helper
{
    internal class Downloader
    {
        public static byte[] DownloadBytes(string url)
        {
            Stream stream = Download(url);
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        public static Stream Download(string url)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            return Download(webRequest);
        }

        public static string DownloadString(string url)
        {
            return DownloadString((HttpWebRequest)WebRequest.Create(url));
        }

        public static string DownloadString(HttpWebRequest request)
        {
            request.SetProxy();
            using (WebResponse response = request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        public static void DownloadStringAsync(string url, DownloadProgressChangedEventHandler progressChangedHandler, DownloadStringCompletedEventHandler completedHandler)
        {
            WebClient client = new WebClient();
            client.SetProxy();
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(progressChangedHandler);
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(completedHandler);
            client.DownloadStringAsync(new Uri(url), url);
        }

        private static Stream Download(HttpWebRequest request)
        {
            int readSize = 0;
            byte[] buffer = new byte[2042];
            MemoryStream memStream = new MemoryStream();

            request.SetProxy();
            using (WebResponse response = request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    while ((readSize = responseStream.Read(buffer, 0, buffer.Length)) != 0)
                        memStream.Write(buffer, 0, readSize);
                }
            }
            memStream.Position = 0;
            return memStream;
        }
    }
}