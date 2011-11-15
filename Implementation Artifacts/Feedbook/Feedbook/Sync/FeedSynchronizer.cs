using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using Feedbook.Services;
using Feedbook.Helper;

namespace Feedbook.Sync
{
    public class FeedSynchronizer : ISynchronizer
    {
        private DispatcherTimer feedSynchronizer = new DispatcherTimer();

        public FeedSynchronizer()
        {
            this.feedSynchronizer.Interval = Constants.SysConfig.SynchronizeInterval;
            this.feedSynchronizer.Tick += new EventHandler(OnTick);
        }

        #region ISynchronizer Members

        public void Start()
        {
            this.feedSynchronizer.Start();
        }

        public void Stop()
        {
            this.feedSynchronizer.Stop();
        }

        #endregion

        private void OnTick(object sender, EventArgs e)
        {
            this.Sychronize();
        }

        private void Sychronize()
        {
            try
            {
                foreach (var channel in DataStore.Channels)
                    SocialService.GetSocialService(null).AsyncUpdate(channel);

                foreach (var account in DataStore.TwitterAccounts)
                {
                    var service = SocialService.GetSocialService(account);
                    foreach (var channel in account.Channels)
                        service.AsyncUpdate(channel);
                }

                foreach (var account in DataStore.GBuzzAccounts)
                {
                    var service = SocialService.GetSocialService(account);
                    foreach (var channel in account.Channels)
                        service.AsyncUpdate(channel);
                }
            }
            catch (Exception ex)
            {
                this.LogAndNotify("Feed Synchronization failed!", ex);
            }
        }

    }
}
