using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using log4net;
using Feedbook.Views;

namespace Feedbook.Helper
{
    internal static class UIElementExtension
    {
        public static void Notify(this UIElement element, object message)
        {
            MainWindow mainWindow;
            if (Application.Current != null && (mainWindow = Application.Current.MainWindow as MainWindow) != null)
                mainWindow.Notify(message);
        }

        public static void Notify(this UIElement element, string message, params object[] arguments)
        {
            MainWindow mainWindow;
            if (Application.Current != null && (mainWindow = Application.Current.MainWindow as MainWindow) != null)
                mainWindow.Notify(string.Format(message, arguments));
        }

        public static void LogAndShow(this UIElement element, string message, Exception exception)
        {
            ILog logger = LogManager.GetLogger(element.GetType());
            logger.Error(message, exception);
            FBMessageBox.Show(message);
        }

        public static void LogAndShowError(this UIElement element, string message, Exception exception)
        {
            ILog logger = LogManager.GetLogger(element.GetType());
            logger.Error(message, exception);
            FBMessageBox.ShowError(message, exception);
        }

        public static void LogAndNotify(this UIElement element, string message, Exception exception)
        {
            ILog logger = LogManager.GetLogger(element.GetType());
            logger.Error(message, exception);
            element.Notify(message);           
        }

        public static void TwitterException(this UIElement element, string message, Exception exception)
        {
            if (exception.HttpStatusCode() == HttpStatusCode.Unauthorized)
            {
                FBMessageBox.Show(
@"Session token store by Feedbook of your twitter account is seems to be expired,
you need to refresh your account from twitter");

                (new TwitterAccount()).ShowDialog();
            }
            else
                element.LogAndShowError(message, exception);
        }

        public static bool IsVisible(this UIElement element)
        {
            return element.Visibility == Visibility.Visible;
        }

        public static void ShowSlow(this UIElement element, int milliseconds)
        {
            if (!element.IsVisible())
            {
                element.UpdateLayout();
                element.Show();
                Duration duration = new Duration(TimeSpan.FromMilliseconds(milliseconds));
                DoubleAnimation linearAnimation = new DoubleAnimation { To = 1, Duration = duration };
                linearAnimation.EasingFunction = new BackEase { EasingMode = EasingMode.EaseInOut };
                element.BeginAnimation(UIElement.OpacityProperty, linearAnimation);
            }
        }

        public static void HideSlow(this UIElement element, int milliseconds)
        {
            if (element.IsVisible())
            {
                Duration duration = new Duration(TimeSpan.FromMilliseconds(milliseconds));
                DoubleAnimation linearAnimation = new DoubleAnimation { To = 0, Duration = duration };
                linearAnimation.EasingFunction = new BackEase { EasingMode = EasingMode.EaseInOut };
                element.BeginAnimation(UIElement.OpacityProperty, linearAnimation);

                element.BeginInvoke(() => element.Hide(), milliseconds);
            }
        }

        public static void Show(this UIElement element, int milliseconds)
        {
            element.ShowSlow(milliseconds);
        }

        public static void Show(this UIElement element, IEasingFunction easingFunction, int milliseconds)
        {
            if (!element.IsVisible())
            {
                var scaleTransform = new ScaleTransform { ScaleX = 0, ScaleY = 0 };
                element.Show();
                element.RenderTransformOrigin = new Point(0, 0);
                element.RenderTransform = scaleTransform;

                Duration duration = new Duration(TimeSpan.FromMilliseconds(milliseconds));

                DoubleAnimation scaleX = new DoubleAnimation { To = 1, Duration = duration };
                scaleX.EasingFunction = easingFunction;

                DoubleAnimation scaleY = new DoubleAnimation { To = 1, Duration = duration };
                scaleY.EasingFunction = easingFunction;

                scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleX);
                scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleY);
            }
        }

        public static void ShowBackEaseInOut(this UIElement element, int milliseconds)
        {
            element.ShowBackEase(EasingMode.EaseInOut, milliseconds);
        }

        public static void ShowBackEaseOut(this UIElement element, int milliseconds)
        {
            element.ShowBackEase(EasingMode.EaseOut, milliseconds);
        }

        public static void ShowBackEaseIn(this UIElement element, int milliseconds)
        {
            element.ShowBackEase(EasingMode.EaseIn, milliseconds);
        }

        public static void ShowBackEase(this UIElement element, EasingMode easingMode, int milliseconds)
        {
            element.Show(new BackEase { EasingMode = easingMode }, milliseconds);
        }

        public static void ShowBounceEase(this UIElement element, EasingMode easingMode, int milliseconds)
        {
            element.Show(new BounceEase { EasingMode = easingMode }, milliseconds);
        }

        public static void ShowBounceEase(this UIElement element, int bounces, double bouciness, EasingMode easingMode, int milliseconds)
        {
            element.Show(new BounceEase { EasingMode = easingMode, Bounces = bounces, Bounciness = bouciness }, milliseconds);
        }

        public static void ShowElasticEase(this UIElement element, EasingMode easingMode, int milliseconds)
        {
            element.Show(new ElasticEase { EasingMode = easingMode }, milliseconds);
        }

        public static void ShowElasticEase(this UIElement element, int oscillations, double springiness, EasingMode easingMode, int milliseconds)
        {
            element.Show(new ElasticEase { EasingMode = easingMode, Oscillations = oscillations, Springiness = springiness }, milliseconds);
        }

        public static void Hide(this UIElement element, int milliseconds)
        {
            element.HideSlow(milliseconds);
        }

        public static void Show(this UIElement element)
        {
            element.Visibility = Visibility.Visible;
        }

        public static void Hide(this UIElement element)
        {
            element.Visibility = Visibility.Hidden;
        }

        public static void Collapsed(this UIElement element)
        {
            element.Visibility = Visibility.Collapsed;
        }

        public static void BeginInvoke(this UIElement element, Action action)
        {
            element.BeginInvoke(action, TimeSpan.Zero);
        }

        public static void BeginInvoke(this UIElement element, Action action, TimeSpan wait)
        {
            Util.BeginInvoke(element.Dispatcher, action, wait);
        }

        public static void BeginInvoke(this UIElement element, Action action, int milliseconds)
        {
            Util.BeginInvoke(element.Dispatcher, action, TimeSpan.FromMilliseconds(milliseconds));
        }
    }
}
