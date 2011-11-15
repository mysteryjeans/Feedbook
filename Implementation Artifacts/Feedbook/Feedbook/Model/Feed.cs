/*
 * Author: Faraz Masood Khan 
 * 
 * Date:  Saturday, July 31, 2010 2:45 AM
 * 
 * Class: Feed
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
using System.Net;
using System.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Web;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using CoreSystem.RefTypeExtension;


namespace Feedbook.Model
{
    [DataContract]
    internal class Feed : Entity
    {
        #region Declarations

        protected string guid = default(string);

        protected string title = default(string);

        protected string commenturl = default(string);

        protected int like = default(int);

        protected bool isliked = default(bool);

        protected bool isprotected = default(bool);

        protected DateTime updated = default(DateTime);

        protected bool isreaded = default(bool);

        protected bool isPodcastDownloading = default(bool);

        protected ObservableCollection<Enclosure> enclosures;

        protected Link link;

        protected string textDescription;

        protected string encodedDescription;

        #endregion

        #region Properties

        [DataMember]
        public string Guid
        {
            get { return this.guid; }
            set
            {
                if (this.guid != value)
                {
                    this.guid = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Guid"));
                }
            }
        }

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
        public string CommentUrl
        {
            get { return this.commenturl; }
            set
            {
                if (this.commenturl != value)
                {
                    this.commenturl = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("CommentUrl"));
                }
            }
        }

        [DataMember]
        public int Likes
        {
            get { return this.like; }
            set
            {
                if (this.like != value)
                {
                    this.like = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Likes"));
                }
            }
        }

        [DataMember]
        public bool IsLiked
        {
            get { return this.isliked; }
            set
            {
                if (this.isliked != value)
                {
                    this.isliked = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("IsLiked"));
                }
            }
        }

        [DataMember]
        public bool IsProtected
        {
            get { return this.isprotected; }
            set
            {
                if (this.isprotected != value)
                {
                    this.isprotected = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("IsProtected"));
                }
            }
        }

        [DataMember]
        public DateTime Updated
        {
            get { return this.updated; }
            set
            {
                if (this.updated != value)
                {
                    this.updated = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Updated"));
                }
            }
        }

        [DataMember]
        public bool IsReaded
        {
            get { return this.isreaded; }
            set
            {
                if (this.isreaded != value)
                {
                    this.isreaded = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("IsReaded"));
                }
            }
        }

        [IgnoreDataMember]
        public bool IsPodcastDownloading
        {
            get { return this.isPodcastDownloading; }
            set
            {
                if (this.isPodcastDownloading != value)
                {
                    this.isPodcastDownloading = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("IsPodcastDownloading"));
                }
            }
        }

        [DataMember]
        public Link Link
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

        public Person Author
        {
            get
            {
                var author = this.People.FirstOrDefault(p => p.PersonRole == PersonRole.Author);
                return author != null ? author : this.People.FirstOrDefault();
            }
        }

        public string TextDescription
        {
            get
            {
                if (this.textDescription == null)
                {
                    try
                    {
                        if (this.Descriptions.Count(content => content.Type == Content.TEXT) > 0)
                            this.textDescription = HttpUtility.HtmlDecode(this.Descriptions.First(content => content.Type == Content.TEXT).Value);
                        else if (this.Descriptions.Count(content => content.Type == Content.XHTML) > 0)
                        {
                            this.textDescription = this.Descriptions.First(content => content.Type == Content.XHTML).ToPlainText;
                            this.textDescription = HttpUtility.HtmlDecode(this.textDescription);
                        }
                        else if (this.Descriptions.Count > 0)
                            this.textDescription = HttpUtility.HtmlDecode(this.Descriptions[0].Value);

                        if (this.textDescription != null)
                            this.textDescription = this.textDescription.Trim('\r', '\n', ' ');
                    }
                    catch { }
                }

                return this.textDescription;
            }
        }

        public string EncodedDescription
        {
            get
            {
                if (this.encodedDescription == null)
                {
                    if (this.Descriptions.Count(content => content.Type == Content.XHTML) > 0)
                        this.encodedDescription = this.Descriptions.Where(content => content.Type == Content.XHTML).First().Value;
                    else if (this.Descriptions.Count(content => content.Type == Content.HTML) > 0)
                        this.encodedDescription = this.Descriptions.Where(content => content.Type == Content.HTML).First().Value;
                    else if (this.Descriptions.Count > 0)
                        this.encodedDescription = this.Descriptions[0].Value;
                }

                return this.encodedDescription;
                //return  "<div style=\"font-family:san-sarif;\">" + this.encodedDescription + "</div>";
            }
        }

        [IgnoreDataMember]
        public Channel Channel { get; set; }

        [DataMember]
        public ObservableCollection<Comment> Comments { get; set; }

        [DataMember]
        public ObservableCollection<Category> Categories { get; set; }

        [DataMember]
        public ObservableCollection<Content> Descriptions { get; set; }

        [DataMember]
        public ObservableCollection<Person> People { get; set; }

        [DataMember]
        public ObservableCollection<Enclosure> Enclosures
        {
            get { return this.enclosures; }
            set
            {
                if (this.enclosures != null)
                {
                    this.enclosures.Clear();
                    if (value != null) this.enclosures.AddRange(value);
                }
            }
        }

        #endregion

        #region Methods

        public Feed()
        {
            this.Initialized(new StreamingContext());
        }

        [OnDeserializing]
        private void Initialized(StreamingContext context)
        {
            this.Comments = new ObservableCollection<Comment>();
            this.Categories = new ObservableCollection<Category>();
            this.Descriptions = new ObservableCollection<Content>();
            this.People = new ObservableCollection<Person>();

            this.enclosures = new ObservableCollection<Enclosure>();
            this.enclosures.CollectionChanged += new NotifyCollectionChangedEventHandler(OnEnclosuresCollectionChanged);
        }

        private void OnEnclosuresCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (Enclosure enclosure in e.NewItems)
                {
                    enclosure.Feed = this;
                    enclosure.PropertyChanged += new PropertyChangedEventHandler(OnEnclosurePropertyChanged);
                }

            if (e.OldItems != null)
                foreach (Enclosure enclosure in e.OldItems)
                {
                    if (enclosure.Feed == this)
                        enclosure.Feed = null;
                    enclosure.PropertyChanged -= new PropertyChangedEventHandler(OnEnclosurePropertyChanged);
                }

            this.IsPodcastDownloading = this.Enclosures.Any(en => en.IsDownloading);
        }

        private void OnEnclosurePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDownloading")
            {
                var enclosure = sender as Enclosure;
                if (enclosure != null && enclosure.IsDownloading)
                    this.IsPodcastDownloading = true;
                else
                    this.IsPodcastDownloading = this.Enclosures.Any(en => en.IsDownloading);
            }
        }

        #endregion
    }
}
