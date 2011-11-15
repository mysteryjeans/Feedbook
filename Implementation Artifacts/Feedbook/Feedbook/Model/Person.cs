/*
 * Author: Faraz Masood Khan 
 * 
 * Date:  Saturday, July 31, 2010 5:17 PM
 * 
 * Class: Person
 * 
 * Email: mk.faraz@gmail.com
 * 
 * Blogs: http://csharplive.wordpress.com, http://farazmasoodkhan.wordpress.com
 *
 * Website: http://www.linkedin.com/in/farazmasoodkhan
 *
 * Copyright: Faraz Masood Khan @ Copyright 2009
 *
/*/

using System;
using System.ComponentModel;
using CoreSystem.ValueTypeExtension;
using System.Runtime.Serialization;

namespace Feedbook.Model
{
    [DataContract]
    internal class Person : Entity
    {
        #region Declarations  

        protected string name = default(string);

        protected string userid = default(string);

        protected string imageurl = default(string);

        protected string url = default(string);

        protected string email = default(string);

        protected string role = default(string);

        protected bool isprotected = default(bool);

        #endregion

        #region Properties   

        [DataMember]
        public string Name
        {
            get { return this.name; }
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Name"));
                }
            }
        }

        [DataMember]
        public string UserId
        {
            get { return this.userid; }
            set
            {
                if (this.userid != value)
                {
                    this.userid = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("UserId"));
                }
            }
        }

        [DataMember]
        public string ImageUrl
        {
            get { return this.imageurl; }
            set
            {
                if (this.imageurl != value)
                {
                    this.imageurl = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("ImageUrl"));
                }
            }
        }

        [DataMember]
        public string Url
        {
            get { return this.url; }
            set
            {
                if (this.url != value)
                {
                    this.url = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Url"));
                }
            }
        }

        [DataMember]
        public string Email
        {
            get { return this.email; }
            set
            {
                if (this.email != value)
                {
                    this.email = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Email"));
                }
            }
        }

        [DataMember]
        public string Role
        {
            get { return this.role; }
            set
            {
                if (this.role != value)
                {
                    this.role = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Role"));
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
        public PersonRole PersonRole
        {
            get { return this.Role.ToEnum<PersonRole>(); }
            set { this.Role = value.ToString(); }
        }

        #endregion      
    }
}
