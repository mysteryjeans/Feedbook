using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Feedbook.Helper;
using System.Windows.Threading;
using Feedbook.Services;
using System.Threading;
using Feedbook.Model;
using System.IO;
using System.Net;
using CoreSystem.ComponentModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Runtime.Serialization;
using Feedbook.Sync;
using System.Deployment.Application;
using System.Text;
using System.Reflection;
using Feedbook.Services.WebFeed;

namespace Feedbook
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    internal partial class FbApplication : Application
    {
        private Mutex mutex;
        private FeedSynchronizer feedSynchronizer;
        private PodcastSynchronizer podcastSychronizer;

        public static FbApplication App { get; private set; }

        public FbApplication()
        {
            App = this;
            this.feedSynchronizer = new FeedSynchronizer();
            this.podcastSychronizer = new PodcastSynchronizer();
        }

        public bool IsStartup { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            if (CheckSingleInstance())
            {
                log4net.Config.XmlConfigurator.Configure();

                CreateStartupShortcut();

                CheckForUpdate();

                SetupFeeds();

                this.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(OnUnhandledException);

                GlobalEventManager.Initialize();
                GlobalEventManager.OnFeedAdd += new FeedEventHandler(OnFeedAdd);

                if (Feedbook.Properties.Settings.Default.CanDownload)
                {
                    this.feedSynchronizer.Start();

                    if (Feedbook.Properties.Settings.Default.CanDownloadPodcast)
                        this.podcastSychronizer.Start();
                }

                this.IsStartup = e.Args != null && e.Args.Length > 0 && "Startup".Equals(e.Args[0], StringComparison.OrdinalIgnoreCase);
                base.OnStartup(e);
            }
        }

        private void SetupFeeds()
        {
            if (ApplicationDeployment.IsNetworkDeployed && ApplicationDeployment.CurrentDeployment.IsFirstRun && DataStore.Channels.Count == 0)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback((object state) =>
                    {
                        foreach (var rssUrl in Constants.Default.PresetRssUrls)
                        {
                            try
                            {
                                var channel = new FeedProcessor().Parse(Downloader.DownloadString(rssUrl), rssUrl);
                                this.Dispatcher.BeginInvoke(new Action(() => DataStore.Channels.Add(channel)));
                            }
                            catch { }
                        }
                    }));
            }
        }

        private void CheckForUpdate()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                var deployment = ApplicationDeployment.CurrentDeployment;

                deployment.CheckForUpdateCompleted += (sender, updateArg) =>
                    {
                        if (!updateArg.Cancelled)
                        {
                            if (updateArg.Error != null)
                                this.Log("Error checking for new version", updateArg.Error);
                            else
                            {
                                if (updateArg.UpdateAvailable)
                                {
                                    deployment.UpdateCompleted += (sender1, e) =>
                                        {
                                            if (!e.Cancelled)
                                                if (e.Error != null)
                                                    this.Log("Error download application update", e.Error);
                                                else
                                                    if (updateArg.IsUpdateRequired)
                                                    {
                                                        FBMessageBox.Show("Application has a mandatory update, it needs to be restarted!", "Application Update");
                                                        System.Windows.Forms.Application.Restart();
                                                    }
                                                    else
                                                        Notification.ShowInfo("Application Update", "Application update is successful!, please restart application to reflect changes.");
                                        };

                                    deployment.UpdateAsync();
                                }
                            }
                        }
                    };

                deployment.CheckForUpdateAsync();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            try
            {
                this.feedSynchronizer.Stop();
                if (this.podcastSychronizer != null)
                    this.podcastSychronizer.Stop();
            }
            finally { DataStore.Save(); }
        }

        public void StartSynchronizingFeeds()
        {
            this.feedSynchronizer.Start();
        }

        public void StopSynchronizingFeeds()
        {
            this.feedSynchronizer.Stop();
        }

        public void StartSynchronizingPodcasts()
        {
            this.podcastSychronizer.Start();
        }

        public void StopSynchronizingPodcasts()
        {
            this.podcastSychronizer.Stop();
        }

        private void OnFeedAdd(object sender, FeedEventArgs e)
        {
            if (Feedbook.Properties.Settings.Default.ShowNotifications)
            {
                switch (e.Source)
                {
                    case FeedSource.RSS:
                        Notification.ShowNotification(Constants.Resources.IMAGE_RSS, e.Feed.Title, e.Feed.TextDescription);
                        break;
                    case FeedSource.Twitter:
                        Notification.ShowNotification(Constants.Resources.IMAGE_TWITTER, e.Feed.Title, e.Feed.TextDescription);
                        break;
                    case FeedSource.GoogleBuzz:
                        Notification.ShowNotification(Constants.Resources.IMAGE_GBUZZ, e.Feed.Title, e.Feed.TextDescription);
                        break;
                }
            }
        }

        private void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            this.Log("Unhandled exception", e.Exception);
            e.Handled = true;
        }

        private bool CheckSingleInstance()
        {
            bool iscreated;
            this.mutex = new Mutex(true, Constants.APPLICATION_NAME + "_" + Environment.UserName, out iscreated);

            if (!iscreated)
            {
                FBMessageBox.Show("Another instance of Feedbook is already running ...");
                this.Shutdown();
                return false;
            }

            return true;
        }

        private void CreateStartupShortcut()
        {
            try
            {
                var productName = System.Windows.Forms.Application.ProductName;
                var publisherName = productName;
                if (ApplicationDeployment.IsNetworkDeployed)
                {
                    try
                    {
                        string startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

                        startupPath = Path.Combine(startupPath, productName + ".appref-ms");

                        if (Feedbook.Properties.Settings.Default.LaunchAtStartup)
                        {
                            if (!File.Exists(startupPath) || ApplicationDeployment.CurrentDeployment.IsFirstRun)
                            {
                                string allProgramsPath = Environment.GetFolderPath(Environment.SpecialFolder.Programs);

                                string shortcutPath = Path.Combine(allProgramsPath, publisherName);

                                shortcutPath = Path.Combine(shortcutPath, productName) + ".appref-ms";

                                File.Copy(shortcutPath, startupPath, true);
                            }
                        }
                        else if (File.Exists(startupPath))
                            File.Delete(startupPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }


                }
            }
            catch (Exception ex)
            {
                this.Log("Failed to configure startup shortcut", ex);
            }
        }
    }
}
