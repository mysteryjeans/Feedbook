using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;

namespace Feedbook.Helper
{
    public class ThumbnailHelper
    {
        public static string GetThumbnailImage(string url)
        {
            WebBrowser CurrentBrowser = new WebBrowser();
            CurrentBrowser.Navigate(new Uri(url, UriKind.Absolute));
            Guid guid = Guid.NewGuid();
            CurrentBrowser.ApplyTemplate();
            string ThumbnailPath = @"E:\" + guid.ToString() + ".png";
            CurrentBrowser.LoadCompleted += (sender, e) =>
                {
                    Image imgScreen = new Image();
                    imgScreen.Width = 120;
                    imgScreen.Height = 100;
                    imgScreen.Source = new DrawingImage(VisualTreeHelper.GetDrawing(CurrentBrowser));

                    FileStream stream = new FileStream(ThumbnailPath, FileMode.Create);

                    DrawingVisual vis = new DrawingVisual();
                    DrawingContext cont = vis.RenderOpen();
                    cont.DrawImage(imgScreen.Source, new Rect(new Size(120d, 100d)));
                    cont.Close();

                    RenderTargetBitmap rtb = new RenderTargetBitmap((int)imgScreen.Width, (int)imgScreen.Height, 96d, 96d, PixelFormats.Default);
                    rtb.Render(vis);

                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(rtb));
                    encoder.Save(stream);
                    stream.Close();
                };

            return ThumbnailPath;
        }
    }
}
