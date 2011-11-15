using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CoreSystem.Util;
using System.Threading;

namespace Feedbook.Helper
{
    internal static class ImageCache
    {
        private static readonly string BASE_FOLDER = Path.Combine(Path.GetTempPath(), @"Feedbook\Cache\Images");

        private static readonly List<string> CachingImage = new List<string>();

        static ImageCache()
        {
            if (!Directory.Exists(BASE_FOLDER))
                Directory.CreateDirectory(BASE_FOLDER);
        }

        public static string GetCacheImage(string imageUri)
        {
            string cacheImage = TryGetCacheImage(imageUri);
            if (cacheImage == null)
                CacheImage(imageUri);

            return (cacheImage = TryGetCacheImage(imageUri)) != null ? cacheImage : imageUri;
        }

        public static Uri GetCacheImageUri(string imageUri)
        {
            string cacheImage = TryGetCacheImage(imageUri);
            if (cacheImage == null)
                CacheImage(imageUri);

            if ((cacheImage = TryGetCacheImage(imageUri)) != null)
                return new Uri(cacheImage, UriKind.Absolute);

            return new Uri(imageUri, imageUri.StartsWith("http://") ? UriKind.Absolute : UriKind.RelativeOrAbsolute);
        }

        public static string TryGetCacheImage(string imageUri)
        {
            string cacheFile = GetFullPath(GetCacheFile(imageUri));

            return File.Exists(cacheFile) && !IsDownloadingImage(imageUri) ? GetFileUri(cacheFile) : null;
        }

        public static void CacheImage(string imageUri)
        {
            string cacheFile = GetCacheFile(imageUri);
            lock (CachingImage)
            {
                if (CachingImage.Contains(imageUri) || File.Exists(cacheFile))
                    return;

                CachingImage.Add(imageUri);
            }

            Thread thread = new Thread(new ThreadStart(delegate
                {
                    string fullPath = GetFullPath(cacheFile);
                    try
                    {
                        byte[] bytes = Downloader.DownloadBytes(imageUri);
                        using (FileStream stream = new FileStream(fullPath, FileMode.CreateNew))
                        {                            
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Close();
                        }
                    }
                    catch
                    {
                        if (File.Exists(fullPath))
                            File.Delete(fullPath);

                        //throw;
                    }
                    finally
                    {
                        lock (CachingImage)
                        {
                            CachingImage.Remove(imageUri);
                        }
                    }
                }));
            thread.Start();
        }

        private static bool IsDownloadingImage(string imageUri)
        {
            lock (CachingImage)
            {
                return CachingImage.Contains(imageUri);
            }
        }

        private static string GetCacheFile(string imageUri)
        {
            Guard.CheckNullOrEmpty(imageUri, "GetCacheImage(imageUri)");

            //var request = System.Net.WebRequest.Create(imageUri);
            //return string.Format("{0}{1}", imageUri.GetHashCode(), Path.GetExtension(request.RequestUri.LocalPath));

            int queryIndex = imageUri.IndexOf('?');
            string ext = queryIndex != -1 ? Path.GetExtension(imageUri.Substring(0, queryIndex)) : Path.GetExtension(imageUri);

            return imageUri.GetHashCode().ToString() + ext;
        }

        private static string GetFullPath(string cacheFile)
        {
            return Path.GetFullPath(Path.Combine(BASE_FOLDER, cacheFile));
        }

        private static string GetFileUri(string cacheFile)
        {
            //return "pack://siteoforigin:,,,/Cache/Images/" + Path.GetFileName(cacheFile);
            return "file:///" + cacheFile.Replace("\\", "/");
        }
    }
}
