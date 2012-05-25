using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Drawing.Imaging;

namespace Feedbook.Web.Services
{
    /// <summary>
    /// Summary description for Thumbnail
    /// </summary>
    public class Thumbnail : IHttpHandler
    {
        private string url;

        private string fileName;

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            if (!string.IsNullOrWhiteSpace(context.Request.QueryString["url"]))
            {
                this.url = context.Request.QueryString["url"];
                this.fileName = GetFileName(context.Request.PhysicalApplicationPath, url);
                //if (!File.Exists(fileName))
                    this.CreatePreview();

                context.Response.ContentType = GetContentType(this.fileName);
                context.Response.WriteFile(this.fileName);
            }
            else
            {
                context.Response.ContentType = "text/plain";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.Write("Url query parameter missing.");
            }
        }

        public void CreatePreview()
        {
            Thread thread = new Thread(new ThreadStart(this.DoWork));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            while (thread.IsAlive) Thread.Sleep(25);
            //thread.Join();
        }

        private void DoWork()
        {
            using (WebBrowser webBrowser = new WebBrowser())
            {
                webBrowser.ScrollBarsEnabled = false;
                webBrowser.Size = new Size(1024, 768);
                webBrowser.ScriptErrorsSuppressed = true;
                webBrowser.NewWindow += (sender, e) => e.Cancel = true;
                webBrowser.Navigate(this.url);

                while (webBrowser.ReadyState != WebBrowserReadyState.Complete)
                    Application.DoEvents();

               
                using (Bitmap bitmap = new Bitmap(1024, 768))
                {
                    webBrowser.BringToFront();
                    webBrowser.DrawToBitmap(bitmap, webBrowser.Bounds);
                    bitmap.Save(this.fileName, ImageFormat.Png);
                }
            }
        }

        private static string GetFileName(string applicationPath, string url)
        {
            return Path.Combine(applicationPath, url.GetHashCode() + ".png");
        }

        private static string GetContentType(string filename)
        {
            return "image/png";
        }
    }
}