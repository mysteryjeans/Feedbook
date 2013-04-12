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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Feedbook.Custom;
using Feedbook.Helper;
using System.Windows.Media.Animation;
using Feedbook.Views;
using System.Windows.Threading;
using System.Media;
using Feedbook.Mail;
using System.IO;
using Feedbook.Logging;

namespace Feedbook
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    internal partial class MainWindow : FbWindow
    {
        private static readonly SoundPlayer soundPlayer = new SoundPlayer(Application.GetResourceStream(new Uri("pack://application:,,,/Feedbook;component/Audio/Error.wav", UriKind.Absolute)).Stream);

        private int lastLogCount;

        private WindowState? lastState;

        private bool isErrorStoryRunning;

        private DispatcherTimer timer;

        private Storyboard ErrorStoryboard;

        private System.Windows.Forms.NotifyIcon notifyIcon;

        public MainWindow()
        {
            InitializeComponent();
            this.ErrorStoryboard = (Storyboard)this.Resources["ErrorStoryboard"];
            this.AdjustConnectionButtons();
            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromSeconds(10);
            this.timer.Tick += new EventHandler(timer_Tick);

            this.Loaded += (sender, e) => timer.Start();
            this.Unloaded += (sender, e) => timer.Stop();
            this.StateChanged += (sender, e) =>
                {
                    if (this.WindowState != System.Windows.WindowState.Minimized)
                        this.lastState = this.WindowState;
                    else if (Feedbook.Properties.Settings.Default.MinimizeToTray)
                        this.MinimizeToTray();
                };

            this.notifyIcon = new System.Windows.Forms.NotifyIcon();
            this.notifyIcon.Visible = true;
            this.notifyIcon.Text = this.Title;
            this.notifyIcon.Icon = Feedbook.Properties.Resources.Icon;
            this.notifyIcon.Click += new EventHandler(OnNotifyIconClick);
            this.notifyIcon.MouseDown += new System.Windows.Forms.MouseEventHandler(notifyIcon_MouseDown);
            this.Closed += (sender, e) => this.notifyIcon.Visible = false;

            var app = Application.Current as FbApplication;
            if (app != null && app.IsStartup)
                this.MinimizeToTray();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (Environment.OSVersion.Version.Major < 6 || !DwmIsCompositionEnabled())
                this.Background = (Brush)this.FindResource("BackgroundBrush");
        }

        private void notifyIcon_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                ContextMenu menu = (ContextMenu)this.FindResource("NotifierContextMenu");
                menu.IsOpen = true;
            }
        }

        private void MinimizeToTray()
        {
            this.Hide();
        }

        private void RestoreWindow()
        {
            this.Show();
            this.Activate();

            if (this.WindowState == WindowState.Minimized)
                this.WindowState = this.lastState != null ? this.lastState.Value : WindowState.Normal;
        }

        private void OnNotifyIconClick(object sender, EventArgs e)
        {
            this.RestoreWindow();
        }

        private void Menu_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BeginErrorStoryboard()
        {
            if (!this.isErrorStoryRunning)
            {
                this.ErrorStoryboard.Begin();
                this.isErrorStoryRunning = true;
            }
        }

        private void StopErrorStoryboard()
        {
            if (this.isErrorStoryRunning)
            {
                this.ErrorStoryboard.Stop();
                this.isErrorStoryRunning = false;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (LogAppender.LogCount > 0)
            {
                if (lastLogCount < LogAppender.LogCount)
                {
                    soundPlayer.Play();

                    this.ErrorButton.Show();
                    //this.BeginErrorStoryboard();
                    this.ErrorStoryboard.Begin();
                }
            }
            else
            {
                //this.StopErrorStoryboard();
                this.ErrorButton.Opacity = 0.001;
                this.ErrorButton.Collapsed();
            }

            lastLogCount = LogAppender.LogCount;
        }

        public void Notify(object content)
        {
            this.NotificationPresenter.Content = content;
            this.ShowElement(this.NotificationBorder, 400);
            this.BeginInvoke(() => this.HideElement(this.NotificationBorder, 300), 5000);
        }

        public void ShowElement(UIElement element, int milliseconds)
        {
            var scaleTransform = new ScaleTransform { ScaleX = 1, ScaleY = 0 };
            element.Show();
            element.RenderTransformOrigin = new Point(0, 0);
            element.RenderTransform = scaleTransform;

            Duration duration = new Duration(TimeSpan.FromMilliseconds(milliseconds));

            DoubleAnimation scaleY = new DoubleAnimation { To = 1, Duration = duration };
            scaleY.EasingFunction = new BounceEase { EasingMode = EasingMode.EaseOut };

            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleY);
        }

        public void HideElement(UIElement element, int milliseconds)
        {
            if (element.IsVisible())
            {
                var scaleTransform = new ScaleTransform { ScaleX = 1, ScaleY = 1 };
                element.RenderTransformOrigin = new Point(0, 0);
                element.RenderTransform = scaleTransform;

                Duration duration = new Duration(TimeSpan.FromMilliseconds(milliseconds));

                DoubleAnimation scaleY = new DoubleAnimation { To = 0, Duration = duration };

                scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleY);

                element.BeginInvoke(() => element.Hide(), milliseconds);
            }
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.CanDownload)
            {
                Properties.Settings.Default.CanDownload = false;
                Properties.Settings.Default.CanDownloadPodcast = false;
            }
            else
                Properties.Settings.Default.CanDownload = true;
            Properties.Settings.Default.Save();

            this.AdjustConnectionButtons();
        }

        private void DownloadPodcastButton_Click(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.CanDownloadPodcast)
                Properties.Settings.Default.CanDownloadPodcast = false;
            else
            {
                Properties.Settings.Default.CanDownload = true;
                Properties.Settings.Default.CanDownloadPodcast = true;
            }
            Properties.Settings.Default.Save();

            this.AdjustConnectionButtons();
        }

        private void NotificationClose_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.NotificationBorder.Hide();
        }

        private void Settings_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            new FeedbookSettings { Owner = this }.ShowDialog();
        }

        private void ErrorButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var utcNow = DateTime.UtcNow;
                var reportFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), string.Format("Feedbook Error Report - {0:yyyyMMdd HHmm}.xml", utcNow));
                File.WriteAllText(reportFile, LogReport.CreateReport());
                var message = new MAPI();
                message.AddRecipientTo(Constants.SysConfig.FeedbookEmail);
                message.AddAttachment(reportFile);
                message.SendMailPopup("Feedbook Error Report (Desktop)", string.Format(@"

Feedbook Error Report (Desktop) 

UTC Time: {0:ddd MM/dd/yyyy hh:mm tt}", utcNow));

                this.ErrorButton.Collapsed();
                this.ErrorButton.Opacity = 0.001;
            }
            catch (Exception ex)
            {
                this.LogAndShowError("WOops! error occurred while sending error report email", ex);
            }
        }

        private void About_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            new About { Owner = this }.ShowDialog();
        }

        private void RSSReader_Click(object sender, RoutedEventArgs e)
        {            
            this.Twitter.Collapsed();
            this.RSSReader.Show();
        }

        private void Twitter_Click(object sender, RoutedEventArgs e)
        {
            this.RSSReader.Collapsed();
            this.Twitter.Show();
        }

        private void AdjustConnectionButtons()
        {
            var app = Application.Current as FbApplication;
            if (app != null)
            {
                if (Properties.Settings.Default.CanDownload)
                {
                    this.DisconnectButtonText.Hide();
                    app.StartSynchronizingFeeds();
                }
                else
                {
                    this.DisconnectButtonText.Show();
                    app.StopSynchronizingFeeds();
                }

                if (Properties.Settings.Default.CanDownloadPodcast)
                {
                    this.DisconnectPodcastButtonText.Hide();
                    app.StartSynchronizingPodcasts();
                }
                else
                {
                    this.DisconnectPodcastButtonText.Show();
                    app.StopSynchronizingPodcasts();
                }
            }
        }
    }
}
