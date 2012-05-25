/*
 * Author: CrystalMapper 
 * 
 * Date:  Thursday, February 17, 2011 8:55 AM
 * 
 * Class: ThumbnailRequest
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
using System.Data.Common;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;

using CoreSystem.Data;

using CrystalMapper;
using CrystalMapper.Data;
using CrystalMapper.Mapping;
using CrystalMapper.Generic;
using CrystalMapper.Generic.Collection;

namespace Feedbook.Web.Model
{
	[Table(TABLE_NAME)]
    public partial class ThumbnailRequest : Entity< ThumbnailRequest>  
    {		
		#region Table Schema
		
        public const string TABLE_NAME = "dbo.ThumbnailRequests";	
     
		public const string COL_USERNAME = "UserName";
		public const string COL_URLHASH = "UrlHash";
		public const string COL_URL = "Url";
		public const string COL_REQUESTCOUNT = "RequestCount";
		public const string COL_UPDATEDON = "UpdatedOn";
		public const string COL_UPDATEDBY = "UpdatedBy";
		public const string COL_CREATEDON = "CreatedOn";
		public const string COL_CREATEDBY = "CreatedBy";
		
        public const string PARAM_USERNAME = "@UserName";	
        public const string PARAM_URLHASH = "@UrlHash";	
        public const string PARAM_URL = "@Url";	
        public const string PARAM_REQUESTCOUNT = "@RequestCount";	
        public const string PARAM_UPDATEDON = "@UpdatedOn";	
        public const string PARAM_UPDATEDBY = "@UpdatedBy";	
        public const string PARAM_CREATEDON = "@CreatedOn";	
        public const string PARAM_CREATEDBY = "@CreatedBy";	
		
        #endregion
		
		#region Queries
		
		private const string SQL_INSERT_THUMBNAILREQUESTS = "INSERT INTO dbo.ThumbnailRequests( [UserName], [UrlHash], [Url], [RequestCount], [UpdatedOn], [UpdatedBy], [CreatedOn], [CreatedBy]) VALUES ( @UserName, @UrlHash, @Url, @RequestCount, @UpdatedOn, @UpdatedBy, @CreatedOn, @CreatedBy);"  ;
		
		private const string SQL_UPDATE_THUMBNAILREQUESTS = "UPDATE dbo.ThumbnailRequests SET  [Url] = @Url, [RequestCount] = @RequestCount, [UpdatedOn] = @UpdatedOn, [UpdatedBy] = @UpdatedBy, [CreatedOn] = @CreatedOn, [CreatedBy] = @CreatedBy WHERE [UserName] = @UserName AND [UrlHash] = @UrlHash";
		
		private const string SQL_DELETE_THUMBNAILREQUESTS = "DELETE FROM dbo.ThumbnailRequests WHERE  [UserName] = @UserName AND [UrlHash] = @UrlHash ";
		
        #endregion
        	  	
        #region Declarations
        
		protected string username = default(string);
	
		protected string urlhash = default(string);
	
		protected string url = default(string);
	
		protected long requestcount = default(long);
	
		protected System.DateTime updatedon = default(System.DateTime);
	
		protected string updatedby = default(string);
	
		protected System.DateTime createdon = default(System.DateTime);
	
		protected string createdby = default(string);
	
		protected User userRef;
	
        #endregion

 		#region Properties	

        [Column( COL_URLHASH, PARAM_URLHASH )]
                              public virtual string UrlHash 
        {
            get { return this.urlhash; }
			set	{ 
                  if(this.urlhash != value)
                    {
                        this.OnPropertyChanging(new PropertyChangingEventArgs("UrlHash"));  
                        this.urlhash = value;                        
                        this.OnPropertyChanged(new PropertyChangedEventArgs("UrlHash"));
                    }   
                }
        }	
		
        [Column( COL_URL, PARAM_URL )]
                              public virtual string Url 
        {
            get { return this.url; }
			set	{ 
                  if(this.url != value)
                    {
                        this.OnPropertyChanging(new PropertyChangingEventArgs("Url"));  
                        this.url = value;                        
                        this.OnPropertyChanged(new PropertyChangedEventArgs("Url"));
                    }   
                }
        }	
		
        [Column( COL_REQUESTCOUNT, PARAM_REQUESTCOUNT, default(long))]
                              public virtual long RequestCount 
        {
            get { return this.requestcount; }
			set	{ 
                  if(this.requestcount != value)
                    {
                        this.OnPropertyChanging(new PropertyChangingEventArgs("RequestCount"));  
                        this.requestcount = value;                        
                        this.OnPropertyChanged(new PropertyChangedEventArgs("RequestCount"));
                    }   
                }
        }	
		
        [Column( COL_UPDATEDON, PARAM_UPDATEDON, typeof(System.DateTime))]
                              public virtual System.DateTime UpdatedOn 
        {
            get { return this.updatedon; }
			set	{ 
                  if(this.updatedon != value)
                    {
                        this.OnPropertyChanging(new PropertyChangingEventArgs("UpdatedOn"));  
                        this.updatedon = value;                        
                        this.OnPropertyChanged(new PropertyChangedEventArgs("UpdatedOn"));
                    }   
                }
        }	
		
        [Column( COL_UPDATEDBY, PARAM_UPDATEDBY )]
                              public virtual string UpdatedBy 
        {
            get { return this.updatedby; }
			set	{ 
                  if(this.updatedby != value)
                    {
                        this.OnPropertyChanging(new PropertyChangingEventArgs("UpdatedBy"));  
                        this.updatedby = value;                        
                        this.OnPropertyChanged(new PropertyChangedEventArgs("UpdatedBy"));
                    }   
                }
        }	
		
        [Column( COL_CREATEDON, PARAM_CREATEDON, typeof(System.DateTime))]
                              public virtual System.DateTime CreatedOn 
        {
            get { return this.createdon; }
			set	{ 
                  if(this.createdon != value)
                    {
                        this.OnPropertyChanging(new PropertyChangingEventArgs("CreatedOn"));  
                        this.createdon = value;                        
                        this.OnPropertyChanged(new PropertyChangedEventArgs("CreatedOn"));
                    }   
                }
        }	
		
        [Column( COL_CREATEDBY, PARAM_CREATEDBY )]
                              public virtual string CreatedBy 
        {
            get { return this.createdby; }
			set	{ 
                  if(this.createdby != value)
                    {
                        this.OnPropertyChanging(new PropertyChangingEventArgs("CreatedBy"));  
                        this.createdby = value;                        
                        this.OnPropertyChanged(new PropertyChangedEventArgs("CreatedBy"));
                    }   
                }
        }	
		
        [Column( COL_USERNAME, PARAM_USERNAME )]
                              public virtual string UserName                
        {
            get
            {
                if(this.userRef == null)
                    return this.username ;
                
                return this.userRef.UserName;            
            }
            set
            {
                if(this.username != value)
                {
                    this.OnPropertyChanging(new PropertyChangingEventArgs("UserName"));                    
                    this.username = value;                    
                    this.OnPropertyChanged(new PropertyChangedEventArgs("UserName"));
                    
                    this.userRef = null;
                }                
            }          
        }	
        
        public User UserRef
        {
            get { 
                    if(this.userRef == null
                       && this.username != default(string)) 
                    {
                        User userQuery = new User {
                                                        UserName = this.username  
                                                        };
                        
                        User[]  users = userQuery.ToList();                        
                        if(users.Length == 1)
                            this.userRef = users[0];                        
                    }
                    
                    return this.userRef; 
                }
			set	{ 
                  if(this.userRef != value)
                    {
                        this.OnPropertyChanging(new PropertyChangingEventArgs("UserRef"));
                        if (this.userRef != null)
                            this.Parents.Remove(this.userRef);                            
                        
                        if((this.userRef = value) != null) 
                        {
                            this.Parents.Add(this.userRef); 
                            this.username = this.userRef.UserName;
                        }
                        else
                        {
		                    this.username = default(string);
                        }
                        this.OnPropertyChanged(new PropertyChangedEventArgs("UserRef"));
                    }   
                }
        }	
		
        
        #endregion        
        
        #region Methods     
		
       public ThumbnailRequest()
        {
        }
        
        public override bool Equals(object obj)
        {
            ThumbnailRequest entity = obj as ThumbnailRequest;           
            
            return (
                    object.ReferenceEquals(this, entity)                    
                    || (
                        entity != null            
                        && this.UserName == entity.UserName
                        && this.UrlHash == entity.UrlHash
                        && this.UserName != default(string)
                        && this.UrlHash != default(string)
                        )
                    );           
        }
        
        public override int GetHashCode()
        {
            
            int hashCode = 7;
            
            hashCode = (11 * hashCode) + this.username.GetHashCode();
            hashCode = (11 * hashCode) + this.urlhash.GetHashCode();
                        
            return hashCode;          
        }
        
		public override void Read(DbDataReader reader)
		{       
			this.username = (string)reader[COL_USERNAME];
			this.urlhash = (string)reader[COL_URLHASH];
			this.url = (string)reader[COL_URL];
			this.requestcount = (long)reader[COL_REQUESTCOUNT];
			this.updatedon = (System.DateTime)reader[COL_UPDATEDON];
			this.updatedby = (string)reader[COL_UPDATEDBY];
			this.createdon = (System.DateTime)reader[COL_CREATEDON];
			this.createdby = (string)reader[COL_CREATEDBY];
            base.Read(reader);
		}
		
		public override bool Create(DataContext dataContext)
        {
            using(DbCommand command  = dataContext.CreateCommand(SQL_INSERT_THUMBNAILREQUESTS))
            {	
				command.Parameters.Add(dataContext.CreateParameter(this.UserName, PARAM_USERNAME));
				command.Parameters.Add(dataContext.CreateParameter(this.UrlHash, PARAM_URLHASH));
				command.Parameters.Add(dataContext.CreateParameter(this.Url, PARAM_URL));
				command.Parameters.Add(dataContext.CreateParameter(this.RequestCount, PARAM_REQUESTCOUNT));
				command.Parameters.Add(dataContext.CreateParameter(this.UpdatedOn, PARAM_UPDATEDON));
				command.Parameters.Add(dataContext.CreateParameter(this.UpdatedBy, PARAM_UPDATEDBY));
				command.Parameters.Add(dataContext.CreateParameter(this.CreatedOn, PARAM_CREATEDON));
				command.Parameters.Add(dataContext.CreateParameter(this.CreatedBy, PARAM_CREATEDBY));
                return (command.ExecuteNonQuery() == 1);
            }
        }

		public override bool Update(DataContext dataContext)
        {
            using(DbCommand command  = dataContext.CreateCommand(SQL_UPDATE_THUMBNAILREQUESTS))
            {							
				command.Parameters.Add(dataContext.CreateParameter(this.UserName, PARAM_USERNAME));
				command.Parameters.Add(dataContext.CreateParameter(this.UrlHash, PARAM_URLHASH));
				command.Parameters.Add(dataContext.CreateParameter(this.Url, PARAM_URL));
				command.Parameters.Add(dataContext.CreateParameter(this.RequestCount, PARAM_REQUESTCOUNT));
				command.Parameters.Add(dataContext.CreateParameter(this.UpdatedOn, PARAM_UPDATEDON));
				command.Parameters.Add(dataContext.CreateParameter(this.UpdatedBy, PARAM_UPDATEDBY));
				command.Parameters.Add(dataContext.CreateParameter(this.CreatedOn, PARAM_CREATEDON));
				command.Parameters.Add(dataContext.CreateParameter(this.CreatedBy, PARAM_CREATEDBY));
			
                return (command.ExecuteNonQuery() == 1);
            }
        }

		public override bool Delete(DataContext dataContext)
        {
            using(DbCommand command  = dataContext.CreateCommand(SQL_DELETE_THUMBNAILREQUESTS))
            {							
				command.Parameters.Add(dataContext.CreateParameter(this.UserName, PARAM_USERNAME));				
				command.Parameters.Add(dataContext.CreateParameter(this.UrlHash, PARAM_URLHASH));				
                return (command.ExecuteNonQuery() == 1);
            }
        }

        #endregion
        
        #region Entity Relationship Functions
        
        #endregion
    }
}
