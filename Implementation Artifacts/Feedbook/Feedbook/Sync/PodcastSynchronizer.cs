using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreSystem.ComponentModel;
using System.ComponentModel;
using Feedbook.Model;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using Feedbook.Helper;

namespace Feedbook.Sync
{
    internal class PodcastSynchronizer : ISynchronizer
    {
        private object podcastThreadLock = new object();
        private Dispatcher dispatcher = Application.Current.Dispatcher;
        private BackgroundWorkerExtended podcastSychronizer;

        #region ISynchronizer Members

        public void Start()
        {
            if (podcastSychronizer == null)
            {
                this.podcastSychronizer = new BackgroundWorkerExtended();
                this.podcastSychronizer.WorkerSupportsCancellation = true;
                this.podcastSychronizer.DoWork += new System.ComponentModel.DoWorkEventHandler(OnDoWork);
            }

            if (!this.podcastSychronizer.IsBusy)
                this.podcastSychronizer.RunWorkerAsync();
        }

        public void Stop()
        {
            if (this.podcastSychronizer != null && this.podcastSychronizer.IsBusy)
                this.podcastSychronizer.Stop();

            this.podcastSychronizer = null;
        }

        #endregion

        private void OnDoWork(object sender, DoWorkEventArgs e)
        {
            lock (this.podcastThreadLock)
            {
                try
                {
                    while (!this.podcastSychronizer.CancellationPending)
                    {
                        Enclosure[] enclosures = null;

                        var query = from channel in DataStore.Channels
                                    from feed in channel.Feeds
                                    from enclosure in feed.Enclosures
                                    orderby enclosure.DownloadPercentage descending, feed.Updated
                                    where (channel.DownloadPodcasts || enclosure.DownloadPodcast == true) 
                                           && !enclosure.IsDownloaded
                                    select enclosure;

                        this.dispatcher.Invoke(new Action(() => enclosures = query.ToArray()));                        

                        foreach (var enclosure in enclosures)
                        {
                            if (this.podcastSychronizer.CancellationPending)
                                return;
                            try
                            {
                                enclosure.Download(this.dispatcher);
                            }
                            catch(ThreadAbortException) { }
                            catch (Exception ex)
                            {
                                ex.Data["thisXml"] = Util.Serialize(enclosure);
                                this.Log("Error occurred downloading podcast", ex);
                            }
                        }

                        this.podcastSychronizer.Sleep(Constants.SysConfig.SynchronizeInterval);
                    }
                }
                catch (ThreadAbortException) { }
                catch (Exception ex)
                {
                    this.dispatcher.Invoke(new Action(() => this.WebRequestException("Podcast download failed!", ex)));
                }
            }
        }
    }
}
