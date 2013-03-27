using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Feedbook.Model;
using Feedbook.Custom;
using Feedbook.Properties;

namespace Feedbook.Views
{
    /// <summary>
    /// Interaction logic for RSSChannelSettings.xaml
    /// </summary>
    internal partial class FeedbookSettings : FbWindow
    {
        public FeedbookSettings()
        {
            this.InitializeComponent();            
            this.cbShowNotifications.IsChecked = Settings.Default.ShowNotifications;
            this.cbMinimizeToTray.IsChecked = Settings.Default.MinimizeToTray;
        }

        private void Close_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DataStore.Save();
            Settings.Default.Save();
            this.Close();
        }

        private void Notification_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            Settings.Default.ShowNotifications = (this.cbShowNotifications.IsChecked == true);
        }

        private void MinimizeToTray_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            Settings.Default.MinimizeToTray = (this.cbMinimizeToTray.IsChecked == true);
        }

        private void cbLaunchAtStartup_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.LaunchAtStartup = (this.cbLaunchAtStartup.IsChecked == true);
        }
    }
}