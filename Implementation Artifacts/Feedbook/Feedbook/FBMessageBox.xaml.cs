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

namespace Feedbook
{
    /// <summary>
    /// Interaction logic for FBMessageBox.xaml
    /// </summary>
    internal partial class FBMessageBox : FbWindow
    {
        private MessageBoxResult msgBoxResult;

        private FBMessageBox()
        {
            InitializeComponent();
            if (this != Application.Current.MainWindow)
                this.Owner = Application.Current.MainWindow;

            this.Loaded += new RoutedEventHandler(FBMessageBox_Loaded);
        }

        private void FBMessageBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Owner == null)
                this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (btnOk == sender)
                this.msgBoxResult = MessageBoxResult.OK;
            else if (btnYes == sender)
                this.msgBoxResult = MessageBoxResult.Yes;
            else if (btnNo == sender)
                this.msgBoxResult = MessageBoxResult.No;
            else
                this.msgBoxResult = MessageBoxResult.Cancel;

            this.Close();
        }

        public static MessageBoxResult Show(string message)
        {
            return Show(message, Constants.APPLICATION_NAME);
        }

        public static MessageBoxResult Show(string message, string caption)
        {
            return Show(message, caption, MessageBoxButton.OK);
        }

        public static MessageBoxResult Show(string message, string caption, MessageBoxButton msgButton)
        {
            FBMessageBox msgBox = new FBMessageBox();
            msgBox.lbMessage.Text = message;
            msgBox.Title = caption;

            switch (msgButton)
            {
                case MessageBoxButton.OK:
                    msgBox.btnOk.Visibility = Visibility.Visible;
                    break;
                case MessageBoxButton.OKCancel:
                    msgBox.btnOk.Visibility = Visibility.Visible;
                    msgBox.btnCancel.Visibility = Visibility.Visible;
                    break;
                case MessageBoxButton.YesNo:
                    msgBox.btnYes.Visibility = Visibility.Visible;
                    msgBox.btnNo.Visibility = Visibility.Visible;
                    break;
                case MessageBoxButton.YesNoCancel:
                    msgBox.btnYes.Visibility = Visibility.Visible;
                    msgBox.btnNo.Visibility = Visibility.Visible;
                    msgBox.btnCancel.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }

            msgBox.ShowDialog();
            return msgBox.msgBoxResult;
        }

        public static MessageBoxResult Show(string message, string reason, string caption, MessageBoxButton msgButtons)
        {
            return FBMessageBox.Show(string.Format("{0}{1}{1}Reason(s):{1}{2}", message, Environment.NewLine, reason), caption, msgButtons);
        }

        public static MessageBoxResult Show(string message, string reason, string caption)
        {
            return FBMessageBox.Show(message, reason, caption, MessageBoxButton.OK);
        }

        private static MessageBoxResult ShowError(string msg, string reason)
        {
            return FBMessageBox.Show(msg, reason, Constants.Caption.ERROR, MessageBoxButton.OK);
        }

        public static MessageBoxResult ShowError(string msg, Exception ex)
        {
            return FBMessageBox.ShowError(msg, FormulateReasons(ex));
        }

        public static MessageBoxResult ShowError(Exception ex)
        {
            if (ex.InnerException != null)
                return FBMessageBox.Show(ex.Message, FormulateReasons(ex.InnerException));

            return FBMessageBox.Show(ex.Message);
        }

        private static string FormulateReasons(Exception ex)
        {
            if (ex.InnerException == null)
                return ex.Message;

            string reasons = "1: " + ex.Message;
            int reasonCount = 2;
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
                reasons += string.Format("{0}{1}: {2}", Environment.NewLine, reasonCount++, ex.Message);
            }

            return reasons;
        }

        private void OnDragWindow(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
