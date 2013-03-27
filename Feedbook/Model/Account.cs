/*
 * Author: Faraz Masood Khan 
 * 
 * Date:  Saturday, July 31, 2010 2:15 AM
 * 
 * Class: Account
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
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;


namespace Feedbook.Model
{
    [DataContract]
    internal class Account : Entity
    {
        #region Declarations

        protected string fullname = default(string);

        protected string username = default(string);

        protected AccountType accounttype = default(AccountType);

        protected string password = default(string);

        protected string imageurl = default(string);

        protected string token = default(string);

        protected string tokensecret = default(string);

        protected bool isDefault = default(bool);

        #endregion

        #region Properties

        [DataMember]
        public string FullName
        {
            get { return this.fullname; }
            set
            {
                if (this.fullname != value)
                {
                    this.fullname = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("FullName"));
                }
            }
        }

        [DataMember]
        public string UserName
        {
            get { return this.username; }
            set
            {
                if (this.username != value)
                {
                    this.username = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("UserName"));
                }
            }
        }

        [DataMember]
        public AccountType AccountType
        {
            get { return this.accounttype; }
            set
            {
                if (this.accounttype != value)
                {
                    this.accounttype = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("AccountType"));
                }
            }
        }

        [DataMember]
        public string Password
        {
            get { return this.password; }
            set
            {
                if (this.password != value)
                {
                    this.password = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Password"));
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
        public string Token
        {
            get { return this.token; }
            set
            {
                if (this.token != value)
                {
                    this.token = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Token"));
                }
            }
        }

        [DataMember]
        public string TokenSecret
        {
            get { return this.tokensecret; }
            set
            {
                if (this.tokensecret != value)
                {
                    this.tokensecret = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("TokenSecret"));
                }
            }
        }

        [DataMember]
        public bool IsDefault
        {
            get { return this.isDefault; }
            set
            {
                if (this.isDefault != value)
                {
                    this.isDefault = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("IsDefault"));
                }
            }
        }

        [DataMember]
        public ObservableCollection<Channel> Channels { get; set; }

        #endregion

        public Account()
        {
            this.Channels = new ObservableCollection<Channel>();
        }
    }
}
