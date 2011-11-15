/*
 * Author: Faraz Masood Khan 
 * 
 * Date:  Saturday, July 31, 2010 5:12 PM
 * 
 * Class: Icon
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
using System.Runtime.Serialization;

namespace Feedbook.Model
{
    [DataContract]
    internal class Icon : Entity
    {
        #region Declarations

        protected string title = default(string);

        protected string description = default(string);

        protected string downloadurl = default(string);

        protected string link = default(string);

        protected int width = default(int);

        protected int height = default(int);

        protected byte[] image = default(byte[]);

        #endregion

        #region Properties

        [DataMember]
        public string Title
        {
            get { return this.title; }
            set
            {
                if (this.title != value)
                {
                    this.title = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Title"));
                }
            }
        }

        [DataMember]
        public string Description
        {
            get { return this.description; }
            set
            {
                if (this.description != value)
                {
                    this.description = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Description"));
                }
            }
        }

        [DataMember]
        public string DownloadUrl
        {
            get { return this.downloadurl; }
            set
            {
                if (this.downloadurl != value)
                {
                    this.downloadurl = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("DownloadUrl"));
                }
            }
        }

        [DataMember]
        public string Link
        {
            get { return this.link; }
            set
            {
                if (this.link != value)
                {
                    this.link = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Link"));
                }
            }
        }

        [DataMember]
        public int Width
        {
            get { return this.width; }
            set
            {
                if (this.width != value)
                {
                    this.width = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Width"));
                }
            }
        }

        [DataMember]
        public int Height
        {
            get { return this.height; }
            set
            {
                if (this.height != value)
                {
                    this.height = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Height"));
                }
            }
        }    

        #endregion
    }
}
