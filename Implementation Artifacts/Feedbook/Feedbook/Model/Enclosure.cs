/*
 * Author: Faraz Masood Khan 
 * 
 * Date:  Saturday, July 31, 2010 2:39 AM
 * 
 * Class: this
 * 
 * Email: mk.faraz@gmail.com
 * 
 * Blogs: http://csharplive.wordpress.com, http://farazmasoodkhan.wordpress.com
 *
 * Website: http://www.linkedin.com/in/farazmasoodkhan
 *
 * Copyright: Faraz Masood Khan @ Copyright 2010
 *
/*/

using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Windows.Threading;
using System.Net.NetworkInformation;
using Feedbook.Helper;


namespace Feedbook.Model
{
    [DataContract]
    internal class Enclosure : Entity
    {
        private static readonly Char[] InvalidChars = Path.GetInvalidFileNameChars();

        #region Declarations

        protected string url = default(string);

        protected string type = default(string);

        protected long? length = default(long?);

        protected string localpath = default(string);

        protected DateTime? downloadedon = default(DateTime?);

        protected bool isdownloaded = default(bool);

        protected bool? downloadPodcast = default(bool?);

        protected bool isdownloading = default(bool);

        protected int downloadPercentage = default(int);

        #endregion

        #region Properties

        [DataMember]
        public string Url
        {
            get { return this.url; }
            set
            {
                if (this.url != value)
                {
                    this.url = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Url"));
                }
            }
        }

        [DataMember]
        public string Type
        {
            get { return this.type; }
            set
            {
                if (this.type != value)
                {
                    this.type = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Type"));
                }
            }
        }

        [DataMember]
        public long? Length
        {
            get { return this.length; }
            set
            {
                if (this.length != value)
                {
                    this.length = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Length"));
                }
            }
        }

        [DataMember]
        public string LocalPath
        {
            get { return this.localpath; }
            set
            {
                if (this.localpath != value)
                {
                    this.localpath = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("LocalPath"));
                }
            }
        }

        [DataMember]
        public DateTime? DownloadedOn
        {
            get { return this.downloadedon; }
            set
            {
                if (this.downloadedon != value)
                {
                    this.downloadedon = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("DownloadedOn"));
                }
            }
        }

        [DataMember]
        public bool IsDownloaded
        {
            get { return this.isdownloaded; }
            set
            {
                if (this.isdownloaded != value)
                {
                    this.isdownloaded = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("IsDownloaded"));
                }
            }
        }

        [DataMember]
        public bool? DownloadPodcast
        {
            get { return this.downloadPodcast; }
            set
            {
                if (this.downloadPodcast != value)
                {
                    this.downloadPodcast = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("DownloadPodcast"));
                }
            }
        }

        [IgnoreDataMember]
        public Feed Feed { get; set; }

        [IgnoreDataMember]
        public bool IsDownloading
        {
            get { return this.isdownloading; }
            set
            {
                if (this.isdownloading != value)
                {
                    this.isdownloading = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("IsDownloading"));
                }
            }
        }

        //[IgnoreDataMember]
        public int DownloadPercentage
        {
            get { return this.downloadPercentage; }
            set
            {
                if (this.downloadPercentage != value)
                {
                    this.downloadPercentage = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("DownloadPercentage"));
                }
            }
        }

        public bool IsAudio
        {
            get { return (this.type != null && this.type.StartsWith("audio")); }
        }

        public bool IsVideo
        {
            get { return (this.type != null && this.type.StartsWith("video")); }
        }

        public bool IsMedia
        {
            get { return this.IsAudio || this.IsVideo; }
        }

        public DateTime Updated
        {
            get { return this.Feed != null ? this.Feed.Updated : DateTime.MinValue; }
        }

        public Channel Channel
        {
            get { return this.Feed != null ? this.Feed.Channel : null; }
        }

        [DataMember]
        public ObservableCollection<Link> Links { get; set; }

        #endregion

        #region Methods

        public Enclosure()
        {
            this.Links = new ObservableCollection<Link>();
        }

        public void Download(Dispatcher dispatcher)
        {
            dispatcher.Invoke(new Action(() =>
                {
                    lock (this)
                    {
                        if (this.IsDownloading)
                            return;
                        this.IsDownloading = true;
                    }
                }));

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this.Url);
                string path = null;

                if (string.IsNullOrEmpty(this.LocalPath))
                {
                    if (this.Feed != null && this.Feed.Channel != null && !string.IsNullOrWhiteSpace(this.Feed.Channel.Title))
                    {
                        var title = new string(this.Feed.Title.Where(c => !InvalidChars.Contains(c)).ToArray());
                        var channelFolder = new string(((string)this.Feed.Channel.Title).Where(c => !InvalidChars.Contains(c)).ToArray());
                        var fileName = string.Format("{0} [{1}]{2}", title, request.RequestUri.LocalPath.GetHashCode(), Path.GetExtension(request.RequestUri.LocalPath));
                        path = Path.Combine(Path.Combine(Constants.SysConfig.DownloadFolder, channelFolder), fileName);
                        dispatcher.Invoke(new Action(() => this.LocalPath = path));
                    }
                    else
                    {
                        path = System.IO.Path.Combine(Constants.SysConfig.DownloadFolder, request.RequestUri.LocalPath.Replace("/", "_"));
                        dispatcher.Invoke(new Action(() => this.LocalPath = path));
                    }
                }
                else
                    path = this.LocalPath;

                if (!Directory.Exists(Path.GetDirectoryName(path)))
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                else if (File.Exists(path))
                {
                    FileInfo downloadFile = new FileInfo(path);
                    if (downloadFile.Length > 0)
                        request.AddRange((int)downloadFile.Length);
                }

                using (WebResponse response = request.GetResponse())
                {
                    // It will store the current number of bytes we retrieved from the server
                    int bytesSize = 0;
                    // A buffer for storing and writing the data retrieved from the server
                    byte[] buffer = new byte[50 * 1048];

                    using (FileStream fileStream = File.OpenWrite(path))
                    {
                        if (this.Length != fileStream.Length + response.ContentLength)
                            dispatcher.Invoke(new Action(() => this.Length = fileStream.Length + response.ContentLength));

                        fileStream.Position = fileStream.Length;
                        if (fileStream.Length != response.ContentLength)
                        {
                            using (Stream responseStream = response.GetResponseStream())
                            {
                                // Loop through the buffer until the buffer is empty
                                while ((bytesSize = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    if (this.downloadPodcast == false)
                                        return;

                                    // Write the data from the buffer to the local hard drive
                                    fileStream.Write(buffer, 0, bytesSize);
                                    fileStream.Flush();
                                    if (this.Length > 0)
                                    {
                                        var downloadPercentage = (int)((fileStream.Length * 100) / this.Length);
                                        dispatcher.Invoke(new Action(() => this.DownloadPercentage = downloadPercentage));
                                    }
                                }
                            }
                        }
                    }
                }

                dispatcher.Invoke(new Action(() =>
                {
                    this.IsDownloaded = true;
                    this.DownloadedOn = DateTime.Now;
                    var title = this.Feed != null ? this.Feed.Title : Path.GetFileName(this.LocalPath);
                    Notification.ShowNotification(Constants.Resources.IMAGE_DOWNLOAD, "Download Completed!", title);
                }));
            }
            catch (WebException ex)
            {
                if (ex.Response != null || NetworkInterface.GetIsNetworkAvailable())
                {
                    ex.Data["thisXml"] = Util.Serialize(this);
                    this.Log("Error occurred downloading podcast", ex);
                }
            }
            finally
            {
                dispatcher.Invoke(new Action(() => this.IsDownloading = false));
            }
        }

        #endregion
    }
}
