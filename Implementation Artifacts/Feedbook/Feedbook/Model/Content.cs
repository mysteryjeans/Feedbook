/*
 * Author: Faraz Masood Khan 
 * 
 * Date:  Saturday, July 31, 2010 2:37 AM
 * 
 * Class: Content
 * 
 * Email: mk.faraz@gmail.com
 * 
 * Blogs: http://csharplive.wordpress.com, http://farazmasoodkhan.wordpress.com
 *
 * Website: http://www.linkedin.com/in/farazmasoodkhan
 *
 * Copyright: Faraz Masood Khan @ Copyright 2010
 *
/*/

using System;
using System.ComponentModel;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Net;
using System.Web;
using System.Runtime.Serialization;


namespace Feedbook.Model
{
    [DataContract]
    internal class Content : Entity
    {
        public const string TEXT = "text";
        public const string HTML = "html";
        public const string XHTML = "xhtml";

        #region Declarations

        protected string value = default(string);

        protected string type = default(string);

        #endregion

        #region Properties

        [DataMember]
        public string Value
        {
            get { return this.value; }
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Value"));
                }
            }
        }

        [DataMember]
        public string Type
        {
            get { return this.type; }
            set
            {
                if (this.type != value)
                {
                    this.type = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Type"));
                }
            }
        }

        public string ToPlainText
        {
            get
            {
                if (this.Type == Content.TEXT)
                    return this.value;

                return Content.ToPainText(this.value);
            }
        }

        #endregion   

        public bool isPainText()
        {
            return (this.Type == Content.TEXT);
        }

        public override string ToString()
        {
            return this.Value;
        }    

        public static bool isValidTextConstructType(string type)
        {

            if (string.IsNullOrEmpty(type))
                return false;

            switch (type.ToLower())
            {
                case TEXT:
                case HTML:
                case XHTML:
                    return true;
                default:
                    return false;
            }
        }

        public static string ToPainText(string contentString)
        {
            if (contentString == null)
                return null;

            try
            {
                if (!contentString.TrimStart().StartsWith("<div>"))
                    contentString = "<div>" + contentString + "</div>";

                return (XElement.Parse(contentString, LoadOptions.PreserveWhitespace)).Value;
            }
            catch
            {
                string clearContent = Regex.Replace(contentString, @"<(.|\n)*?>", string.Empty);
                try { return HttpUtility.HtmlDecode(clearContent); }
                catch { return clearContent; }
            }
        }

        public static string ToPainTextWithoutDecode(string contentString)
        {
            if (contentString == null)
                return null;

            try
            {
                if (!contentString.TrimStart().StartsWith("<div>"))
                    contentString = "<div>" + contentString + "</div>";

                return (XElement.Parse(contentString, LoadOptions.PreserveWhitespace)).Value;
            }
            catch
            {
                return Regex.Replace(contentString, @"<(.|\n)*?>", string.Empty);
            }
        }

        public static string DetermineType(string contentString)
        {
            if (Regex.IsMatch(contentString, Constants.XML_TAG_REGEX))
                return Content.XHTML;

            return Content.TEXT;
        }    

        public static implicit operator Content(string value)
        {
            return new Content
            {
                Value = value,
                Type = Content.TEXT
            };
        }

        public static implicit operator string(Content value)
        {
            return (value != null) ? value.Value : null;
        }
    }
}
