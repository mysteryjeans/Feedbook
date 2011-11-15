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

namespace Feedbook.Views
{
    /// <summary>
    /// Interaction logic for RSSChannelSettings.xaml
    /// </summary>
    internal partial class RSSChannelSettings : FbWindow
    {
        public Channel Channel
        {
            set { this.Border.DataContext = value; }
        }

        public RSSChannelSettings()
        {
            this.InitializeComponent();

            // Insert code required on object creation below this point.
        }

        private void Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var channel = this.Border.DataContext as Channel;
            if (channel != null)
            {
                DataStore.Channels.Remove(channel);
                DataStore.Save();
            }

            this.Close();
        }

        private void Close_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DataStore.Save();
            this.Close();
        }
    }
}