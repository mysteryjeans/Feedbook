/*
 * Author: Faraz Masood Khan 
 * 
 * Date:  Saturday, July 31, 2010 2:21 AM
 * 
 * Class: Channel
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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Collections.Specialized;
using CoreSystem.RefTypeExtension;

namespace Feedbook.Model
{
    [DataContract]
    internal class Channel : Entity
    {
        #region Declarations

        protected string channelId = default(string);

        protected FeedType feedtype = default(FeedType);

        protected string downloadUrl = default(string);

        protected string language = default(string);

        protected string copyright = default(string);

        protected DateTime updated = default(DateTime);

        protected DateTime published = default(DateTime);

        protected int rating = default(int);

        protected string generator = default(string);

        protected int ttl = default(int);

        protected bool downloadpodcast = default(bool);

        protected bool isSynchronizing;

        protected int newFeedCount;

        protected Content title;

        protected Content description;

        protected Icon icon;

        private ObservableCollection<Feed> feeds;

        #endregion

        #region Properties

        [DataMember]
        public string ChannelId
        {
            get { return this.channelId; }
            set
            {
                if (this.channelId != value)
                {
                    this.channelId = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("ChannelId"));
                }
            }
        }

        [DataMember]
        public FeedType FeedType
        {
            get { return this.feedtype; }
            set
            {
                if (this.feedtype != value)
                {
                    this.feedtype = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("FeedType"));
                }
            }
        }

        [DataMember]
        public string DownloadUrl
        {
            get { return this.downloadUrl; }
            set
            {
                if (this.downloadUrl != value)
                {
                    this.downloadUrl = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("DownloadUrl"));
                }
            }
        }

        [DataMember]
        public string Language
        {
            get { return this.language; }
            set
            {
                if (this.language != value)
                {
                    this.language = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Language"));
                }
            }
        }

        [DataMember]
        public string Copyrights
        {
            get { return this.copyright; }
            set
            {
                if (this.copyright != value)
                {
                    this.copyright = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Copyrights"));
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
        public DateTime Published
        {
            get { return this.published; }
            set
            {
                if (this.published != value)
                {
                    this.published = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Published"));
                }
            }
        }

        [DataMember]
        public int Rating
        {
            get { return this.rating; }
            set
            {
                if (this.rating != value)
                {
                    this.rating = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Rating"));
                }
            }
        }

        [DataMember]
        public string Generator
        {
            get { return this.generator; }
            set
            {
                if (this.generator != value)
                {
                    this.generator = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Generator"));
                }
            }
        }

        [DataMember]
        public int Ttl
        {
            get { return this.ttl; }
            set
            {
                if (this.ttl != value)
                {
                    this.ttl = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Ttl"));
                }
            }
        }

        [DataMember]
        public bool DownloadPodcasts
        {
            get { return this.downloadpodcast; }
            set
            {
                if (this.downloadpodcast != value)
                {
                    this.downloadpodcast = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("DownloadPodcasts"));
                }
            }
        }

        [DataMember]
        public Content Title
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
        public Content Description
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
        public Icon Icon
        {
            get { return this.icon; }
            set
            {
                if (this.icon != value)
                {
                    this.icon = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Icon"));
                }
            }
        }

        [IgnoreDataMember]
        public bool IsSynchronizing
        {
            get { return this.isSynchronizing; }
            set
            {
                if (this.isSynchronizing != value)
                {
                    this.isSynchronizing = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("IsSynchronizing"));
                }
            }
        }

        [IgnoreDataMember]
        public int NewFeedCount
        {
            get { return this.newFeedCount; }
            set
            {
                if (this.newFeedCount != value)
                {
                    this.newFeedCount = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("NewFeedCount"));
                }
            }
        }

        public bool IsTwitterSearch
        {
            get
            {
                string downloadUrl = this.DownloadUrl;
                return downloadUrl != null && downloadUrl.StartsWith("http://search.twitter.com/search.atom");
            }
        }

        [DataMember]
        public ObservableCollection<Feed> Feeds
        {
            get { return this.feeds; }
            set
            {
                this.feeds.Clear();
                if (value != null)
                    this.feeds.AddRange(value);
            }
        }

        [DataMember]
        public ObservableCollection<DayOfWeek> SkipDays { get; set; }

        [DataMember]
        public ObservableCollection<int> SkipHours { get; set; }

        [DataMember]
        public ObservableCollection<Category> Categories { get; set; }

        [DataMember]
        public ObservableCollection<Link> Links { get; set; }

        [DataMember]
        public ObservableCollection<Person> People { get; set; }

        #endregion

        #region Methods

        public Channel()
        {
            this.Initialized(new StreamingContext());
        }

        [OnDeserializing]
        private void Initialized(StreamingContext context)
        {
            this.SkipDays = new ObservableCollection<DayOfWeek>();
            this.SkipHours = new ObservableCollection<int>();
            this.Categories = new ObservableCollection<Category>();
            this.Links = new ObservableCollection<Link>();
            this.People = new ObservableCollection<Person>();

            this.feeds = new ObservableCollection<Feed>();
            this.feeds.CollectionChanged += (sender, e) =>
            {
                if (e.NewItems != null)
                    foreach (Feed feed in e.NewItems)
                        feed.Channel = this;

                if (e.OldItems != null)
                    foreach (Feed feed in e.OldItems)
                        feed.Channel = null;
            };
        }

        #endregion
    }
}
