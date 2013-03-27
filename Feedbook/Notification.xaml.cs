using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Media.Animation;
using System.Media;
using Feedbook.Helper;

namespace Feedbook
{
    /// <summary>
    /// Interaction logic for Notification.xaml
    /// </summary>
    internal partial class Notification : Window
    {
        class Message
        {
            public string Icon { get; set; }
            public string Caption { get; set; }
            public string Text { get; set; }
        }

        private static Notification currentNotication;
        private static readonly SoundPlayer soundPlayer = new SoundPlayer(Application.GetResourceStream(new Uri("pack://application:,,,/Feedbook;component/Audio/Download.wav", UriKind.Absolute)).Stream);

        private Storyboard notificationStory;
        private ObservableCollection<Message> messages;

        public Notification()
        {
            InitializeComponent();
            this.notificationStory = (Storyboard)this.Resources["NotificationStory"];
            this.messages = new ObservableCollection<Message>();

            this.Loaded += (sender, e) =>
            {
                this.notificationStory.Completed += (sender1, e1) => this.Close();
                this.notificationStory.Begin();
                this.Left = SystemParameters.WorkArea.Right - this.ActualWidth;
                this.Top = SystemParameters.WorkArea.Bottom - this.ActualHeight;
                soundPlayer.Play();
            };

            this.DataContextChanged += (sender, e) => this.SetDisplayFeedIndex();
            this.messages.CollectionChanged += (sender, e) => this.SetDisplayFeedIndex();

        }

        public static void ShowInfo(string caption, string message)
        {
            ShowNotification(Constants.Resources.IMAGE_INFO, caption, message);
        }

        public static void ShowNotification(string icon, string caption, string message)
        {
            if (currentNotication != null)
                currentNotication.messages.Add(new Message { Icon = icon, Caption = caption, Text = message });
            else
            {
                currentNotication = new Notification();
                currentNotication.Closed += (sender, e) => currentNotication = null;
                currentNotication.messages.Add(new Message { Icon = icon, Caption = caption, Text = message });
                currentNotication.Show();
            }
        }

        private void SetDisplayFeedIndex()
        {
            var message = this.DataContext as Message;
            if (message != null)
            {
                var indexOf = this.messages.IndexOf(message);
                this.FeedTextBlock.Text = string.Format("{0} of {1}", indexOf + 1, this.messages.Count);
            }
            else
                this.DataContext = this.messages.FirstOrDefault();
        }

        private void Previous(object sender, System.Windows.RoutedEventArgs e)
        {
            var message = this.DataContext as Message;
            if (message != null)
            {
                var indexOf = this.messages.IndexOf(message);
                if (indexOf != -1 && indexOf > 0)
                    this.DataContext = this.messages[--indexOf];
            }
        }

        private void Next(object sender, System.Windows.RoutedEventArgs e)
        {
            var message = this.DataContext as Message;
            if (message != null)
            {
                var indexOf = this.messages.IndexOf(message);
                if (indexOf != -1 && (++indexOf) < this.messages.Count)
                    this.DataContext = this.messages[indexOf];
            }
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!notificationStory.GetIsPaused())
                this.notificationStory.Pause();
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.notificationStory.GetIsPaused())
                this.notificationStory.Resume();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.notificationStory.Stop();
            this.Close();
        }

        private void SummaryText_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Util.OpenInBrowser(e.Uri.ToString());
        }
    }
}
