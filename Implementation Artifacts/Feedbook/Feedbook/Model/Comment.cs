/*
 * Author: Faraz Masood Khan 
 * 
 * Date:  Saturday, July 31, 2010 2:33 AM
 * 
 * Class: Comment
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
    internal class Comment : Entity
    {
        #region Declarations

        protected string guid = default(string);

        protected string content = default(string);

        protected DateTime published = default(DateTime);

        protected Feed feed;

        protected Person person;

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
        public string Content
        {
            get { return this.content; }
            set
            {
                if (this.content != value)
                {
                    this.content = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Content"));
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
        public Person Person
        {
            get { return this.person; }
            set
            {
                if (this.person != value)
                {
                    this.person = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Person"));
                }
            }
        }

        #endregion  
    }
}
