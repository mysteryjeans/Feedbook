using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using Feedbook.Helper;

namespace Feedbook.Custom
{
    internal class LinkButton : Button
    {
        public static readonly DependencyProperty NavigateUriProperty = DependencyProperty.Register("NavigateUri", typeof(string), typeof(LinkButton));

        public string NavigateUri
        {
            get { return (string)this.GetValue(NavigateUriProperty); }

            set { this.SetValue(NavigateUriProperty, value); }
        }

        public string TargetName { get; set; }

        protected override void OnClick()
        {
            if (!string.IsNullOrEmpty(this.TargetName))
            {
                var frame = this.FindName(this.TargetName) as Frame;
                if (frame != null)
                    frame.NavigationService.Navigate(new Uri(this.NavigateUri, UriKind.RelativeOrAbsolute));
            }
            else
                Util.OpenInBrowser(this.NavigateUri);
        }
    }
}
