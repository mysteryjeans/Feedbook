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
using System.Diagnostics;
using Feedbook.Custom;
using System.Reflection;

namespace Feedbook
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    internal partial class About : FbWindow
    {		
		public string Version
		{
			get 
			{
				return Assembly.GetExecutingAssembly().GetName().Version.ToString();
			}
		}
		
        public About()
        {
            this.InitializeComponent();

            // Insert code required on object creation below this point.
        }
  
        private void OnDragWindow(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            if (e.Uri == null)
                return;
            try
            {
                Process.Start(e.Uri.ToString());
            }
            catch (Exception ex)
            {
                FBMessageBox.Show(ex.ToString());
               // FBMessageBox.ShowError("WOops! something goes wrong when opening link", ex);
            }
        }

        private void CloseButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	this.Close();
        }
    }
}