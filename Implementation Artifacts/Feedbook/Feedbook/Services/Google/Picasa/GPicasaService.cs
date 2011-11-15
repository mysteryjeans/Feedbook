using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Feedbook.Services.Security;
using Feedbook.Helper;
using CoreSystem.Util;
using System.Xml.Linq;
using System.IO;
using Feedbook.Services.WebFeed;

namespace Feedbook.Services.Google.Picasa
{
    internal class GPicasaService : GoogleService
    {
        public const string NAMESPACE_GOOGLE_PHOTOS = "http://schemas.google.com/photos/2007";

        public const string PICASA_SCOPE_URL = "http://picasaweb.google.com/data/";

        public const string PICASA_WEB_ALBUMS_URL = "http://picasaweb.google.com/data/feed/api/user";

        public static readonly XNamespace gphotosNS = XNamespace.Get(NAMESPACE_GOOGLE_PHOTOS);

        public override string ScopeUrl
        {
            get { return PICASA_SCOPE_URL; }
        }

        public static string GetAlbums(string userId, string token, string tokenSecret)
        {
            Guard.CheckNullOrEmpty(userId, "GetAlbums(userId)");
            string url = string.Format("{0}/{1}", PICASA_WEB_ALBUMS_URL, userId);
            return OAuthGetWebRequest(url, token, tokenSecret);
        }

        public static string CreateAlbum(string userId, string token, string tokenSecret)
        {
            return CreateAlbum(userId, DateTime.Now.ToString("MMMM dd, yyyy"), null, null, AlbumType.Private, token, tokenSecret);
        }

        public static string CreateAlbum(string userId, string title, string summary, string photoLocation, AlbumType albumType, string token, string tokenSecret)
        {            
            Guard.CheckNullOrEmpty(userId, "CreateAlbum(userId)");
            Guard.CheckNullOrEmpty(title, "CreateAlbum(title)");

            var atomEntry = string.Format(
@"<entry xmlns='http://www.w3.org/2005/Atom'
  xmlns:media='http://search.yahoo.com/mrss/'
  xmlns:gphoto='http://schemas.google.com/photos/2007'>
  <title type='text'>{0}</title>
  <summary type='text'>{1}</summary>
  <gphoto:location>{2}</gphoto:location>
  <gphoto:access>{3}</gphoto:access>
  <gphoto:timestamp>{4}</gphoto:timestamp>
  <category scheme='http://schemas.google.com/g/2005#kind' term='http://schemas.google.com/photos/2007#album'></category>
</entry>",         title, summary, photoLocation, albumType.ToString().ToLower(), OAuthHelper.GenerateTimeStamp());

            string url = string.Format("{0}/{1}", PICASA_WEB_ALBUMS_URL, userId);

            var entryEntry = OAuthWebRequest(HttpMethod.POST, url, new PostData(atomEntry, "application/atom+xml"), token, tokenSecret);

            return FeedProcessor.GetMandatoryElementString(XElement.Parse(entryEntry), gphotosNS + FeedProcessor.XML_ID);
        }

        public static string UploadPhoto(string userId, string albumId, string imageTitle, Stream imageStream, PicasaImageFormat imageFormat, string token, string tokenSecret)
        {
            Guard.CheckNullOrEmpty(userId, "UploadPhoto(userId)");
            Guard.CheckNullOrEmpty(albumId, "UploadPhoto(albumId)");
            Guard.CheckNullOrEmpty(imageTitle, "UploadPhoto(imageTitle)");
            Guard.CheckNull(imageStream, "UploadPhoto(imageStream)");

            var postData = new PostData(imageStream, "image/" + imageFormat.ToString().ToLower());
            postData.Headers.Add("Slug", imageTitle);

            string url = string.Format("{0}/{1}/albumid/{2}", PICASA_WEB_ALBUMS_URL, userId, albumId);
            var atomEntry = OAuthWebRequest(HttpMethod.POST, url, postData, token, tokenSecret);

            var content = FeedProcessor.GetMandatoryElement(XElement.Parse(atomEntry), FeedProcessor.atomNS + FeedProcessor.XML_CONTENT);
            return FeedProcessor.GetMandatoryAttributeString(content, FeedProcessor.XML_SRC);
        }
    }
}
