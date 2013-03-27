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
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Feedbook.Helper;
using Feedbook.Model;

namespace Feedbook.Custom
{
    /// <summary>
    /// Interaction logic for MediaPlayer.xaml
    /// </summary>
    internal partial class MediaPlayer : UserControl
    {
        bool isDragging = false;
        private DispatcherTimer timer;
        private bool isPlaying;

        public MediaPlayer()
        {
            InitializeComponent();
            IsPlaying(false);
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Tick += new EventHandler(timer_Tick);
        }

        public void Play()
        {
            this.mediaElement.Play();
            this.timelineSlider.Value = 0;
            IsPlaying(true);
        }

        public void Stop()
        {
            this.TimelinePanel.HideSlow(1000);
            this.BeginInvoke(() => this.TimelinePanel.Collapsed(), 1000);
            this.timer.Stop();
            this.mediaElement.Stop();
            IsPlaying(false);
        }

        private void mediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {            
            this.timelineSlider.Value = 0;
            if (this.mediaElement.NaturalDuration.HasTimeSpan)
            {
                TimeSpan timespan = this.mediaElement.NaturalDuration.TimeSpan;
                this.timelineSlider.Maximum = timespan.TotalSeconds;
                this.timelineSlider.SmallChange = 1;
                this.timelineSlider.LargeChange = Math.Min(10, timespan.Seconds / 10);
                this.MovieDurationText.Text = timespan.ToString(@"hh\:mm\:ss");
            }            
            IsPlaying(true);
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (!this.isPlaying)
            {
                this.mediaElement.Play();
                IsPlaying(true);
            }
            else
            {
                this.mediaElement.Pause();
                IsPlaying(false);
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            this.Stop();
        }

        private void timelineSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            isDragging = true;
        }

        private void timelineSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            isDragging = false;
            this.mediaElement.Position = TimeSpan.FromSeconds(this.timelineSlider.Value);
        }

        private void IsPlaying(bool bValue)
        {
            this.isPlaying = bValue;
            timelineSlider.IsEnabled = bValue;
            this.btnPlay.Content = bValue ? ";" : "4";

            if (bValue)
            {
                this.timer.Start();
                this.TimelinePanel.ShowSlow(1000);
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (!isDragging)
            {
                this.timelineSlider.Value = this.mediaElement.Position.TotalSeconds;
                this.MovieTimelineText.Text = this.mediaElement.Position.ToString(@"hh\:mm\:ss");
            }
        }

        private void btnOpenMedia_Click(object sender, RoutedEventArgs e)
        {
            if (this.mediaElement.Source != null)
            {
                try
                {
                    btnStop_Click(sender, e);
                    System.Diagnostics.Process.Start(this.mediaElement.Source.ToString());
                }
                catch (Exception ex)
                {
                    this.LogAndShowError("WOops! error occurred while openning media in external player ...", ex);
                }
            }
        }

        private void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            this.timer.Stop();
            this.TimelinePanel.HideSlow(1000);
            this.BeginInvoke(() => this.TimelinePanel.Collapsed(), 1000);
        }
    }
}
