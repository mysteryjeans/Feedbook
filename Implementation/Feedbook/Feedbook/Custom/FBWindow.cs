using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Media;

namespace Feedbook.Custom
{
    internal class FbWindow : Window
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int cxLeftWidth;      // width of left border that retains its size
            public int cxRightWidth;     // width of right border that retains its size
            public int cyTopHeight;      // height of top border that retains its size
            public int cyBottomHeight;   // height of bottom border that retains its size
        };


        [DllImport("DwmApi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS pMarInset);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern bool DwmIsCompositionEnabled();

        public override void OnApplyTemplate()
        {
            if (Environment.OSVersion.Version.Major >= 6 && DwmIsCompositionEnabled())
            {
                //this.SetValue(Microsoft.Windows.Shell.WindowChrome.WindowChromeProperty, new WindowChrome { GlassFrameThickness = new Thickness(-1), CaptionHeight = 25 });
                //this.Margin = new Thickness(5);
                ExtendGlass(this, new Thickness(-1));
                this.Background = null;
            }
            base.OnApplyTemplate();
        }

        /// <summary>
        /// Extends the glass area into the client area of the window
        /// </summary>
        /// <param name="window"></param>
        /// <param name="top"></param>
        public static void ExtendGlass(Window window, Thickness thikness)
        {
            try
            {
                // Get the window handle
                WindowInteropHelper helper = new WindowInteropHelper(window);
                HwndSource mainWindowSrc = (HwndSource)HwndSource.FromHwnd(helper.Handle);
                mainWindowSrc.CompositionTarget.BackgroundColor = Colors.Transparent;

                // Get the dpi of the screen
                System.Drawing.Graphics desktop = System.Drawing.Graphics.FromHwnd(mainWindowSrc.Handle);
                float dpiX = desktop.DpiX / 96;
                float dpiY = desktop.DpiY / 96;

                // Set Margins
                MARGINS margins = new MARGINS();
                margins.cxLeftWidth = (int)(thikness.Left * dpiX);
                margins.cxRightWidth = (int)(thikness.Right * dpiX);
                margins.cyBottomHeight = (int)(thikness.Bottom * dpiY);
                margins.cyTopHeight = (int)(thikness.Top * dpiY);

                window.Background = Brushes.Transparent;

                int hr = DwmExtendFrameIntoClientArea(mainWindowSrc.Handle, ref margins);
            }
            catch (DllNotFoundException) { }
        }
    }
}
