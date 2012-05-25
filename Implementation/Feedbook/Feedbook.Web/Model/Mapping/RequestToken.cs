/*
 * Author: CrystalMapper 
 * 
 * Date:  Thursday, February 17, 2011 8:55 AM
 * 
 * Class: RequestToken
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
    public partial class RequestToken : Entity< RequestToken>  
    {		
		#region Table Schema
		
        public const string TABLE_NAME = "dbo.RequestTokens";	
     
		public const string COL_USERNAME = "UserName";
		public const string COL_TOKENHASH = "TokenHash";
		public const string COL_EXPIREDON = "ExpiredOn";
		public const string COL_CREATEDON = "CreatedOn";
		public const string COL_CREATEDBY = "CreatedBy";
		
        public const string PARAM_USERNAME = "@UserName";	
        public const string PARAM_TOKENHASH = "@TokenHash";	
        public const string PARAM_EXPIREDON = "@ExpiredOn";	
        public const string PARAM_CREATEDON = "@CreatedOn";	
        public const string PARAM_CREATEDBY = "@CreatedBy";	
		
        #endregion
		
		#region Queries
		
		private const string SQL_INSERT_REQUESTTOKENS = "INSERT INTO dbo.RequestTokens( [UserName], [TokenHash], [ExpiredOn], [CreatedOn], [CreatedBy]) VALUES ( @UserName, @TokenHash, @ExpiredOn, @CreatedOn, @CreatedBy);"  ;
		
		private const string SQL_UPDATE_REQUESTTOKENS = "UPDATE dbo.RequestTokens SET  [ExpiredOn] = @ExpiredOn, [CreatedOn] = @CreatedOn, [CreatedBy] = @CreatedBy WHERE [UserName] = @UserName AND [TokenHash] = @TokenHash";
		
		private const string SQL_DELETE_REQUESTTOKENS = "DELETE FROM dbo.RequestTokens WHERE  [UserName] = @UserName AND [TokenHash] = @TokenHash ";
		
        #endregion
        	  	
        #region Declarations
        
		protected string username = default(string);
	
		protected string tokenhash = default(string);
	
		protected System.DateTime expiredon = default(System.DateTime);
	
		protected System.DateTime createdon = default(System.DateTime);
	
		protected string createdby = default(string);
	
		protected User userRef;
	
        #endregion

 		#region Properties	

        [Column( COL_TOKENHASH, PARAM_TOKENHASH )]
                              public virtual string TokenHash 
        {
            get { return this.tokenhash; }
			set	{ 
                  if(this.tokenhash != value)
                    {
                        this.OnPropertyChanging(new PropertyChangingEventArgs("TokenHash"));  
                        this.tokenhash = value;                        
                        this.OnPropertyChanged(new PropertyChangedEventArgs("TokenHash"));
                    }   
                }
        }	
		
        [Column( COL_EXPIREDON, PARAM_EXPIREDON, typeof(System.DateTime))]
                              public virtual System.DateTime ExpiredOn 
        {
            get { return this.expiredon; }
			set	{ 
                  if(this.expiredon != value)
                    {
                        this.OnPropertyChanging(new PropertyChangingEventArgs("ExpiredOn"));  
                        this.expiredon = value;                        
                        this.OnPropertyChanged(new PropertyChangedEventArgs("ExpiredOn"));
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
		
       public RequestToken()
        {
        }
        
        public override bool Equals(object obj)
        {
            RequestToken entity = obj as RequestToken;           
            
            return (
                    object.ReferenceEquals(this, entity)                    
                    || (
                        entity != null            
                        && this.UserName == entity.UserName
                        && this.TokenHash == entity.TokenHash
                        && this.UserName != default(string)
                        && this.TokenHash != default(string)
                        )
                    );           
        }
        
        public override int GetHashCode()
        {
            
            int hashCode = 7;
            
            hashCode = (11 * hashCode) + this.username.GetHashCode();
            hashCode = (11 * hashCode) + this.tokenhash.GetHashCode();
                        
            return hashCode;          
        }
        
		public override void Read(DbDataReader reader)
		{       
			this.username = (string)reader[COL_USERNAME];
			this.tokenhash = (string)reader[COL_TOKENHASH];
			this.expiredon = (System.DateTime)reader[COL_EXPIREDON];
			this.createdon = (System.DateTime)reader[COL_CREATEDON];
			this.createdby = (string)reader[COL_CREATEDBY];
            base.Read(reader);
		}
		
		public override bool Create(DataContext dataContext)
        {
            using(DbCommand command  = dataContext.CreateCommand(SQL_INSERT_REQUESTTOKENS))
            {	
				command.Parameters.Add(dataContext.CreateParameter(this.UserName, PARAM_USERNAME));
				command.Parameters.Add(dataContext.CreateParameter(this.TokenHash, PARAM_TOKENHASH));
				command.Parameters.Add(dataContext.CreateParameter(this.ExpiredOn, PARAM_EXPIREDON));
				command.Parameters.Add(dataContext.CreateParameter(this.CreatedOn, PARAM_CREATEDON));
				command.Parameters.Add(dataContext.CreateParameter(this.CreatedBy, PARAM_CREATEDBY));
                return (command.ExecuteNonQuery() == 1);
            }
        }

		public override bool Update(DataContext dataContext)
        {
            using(DbCommand command  = dataContext.CreateCommand(SQL_UPDATE_REQUESTTOKENS))
            {							
				command.Parameters.Add(dataContext.CreateParameter(this.UserName, PARAM_USERNAME));
				command.Parameters.Add(dataContext.CreateParameter(this.TokenHash, PARAM_TOKENHASH));
				command.Parameters.Add(dataContext.CreateParameter(this.ExpiredOn, PARAM_EXPIREDON));
				command.Parameters.Add(dataContext.CreateParameter(this.CreatedOn, PARAM_CREATEDON));
				command.Parameters.Add(dataContext.CreateParameter(this.CreatedBy, PARAM_CREATEDBY));
			
                return (command.ExecuteNonQuery() == 1);
            }
        }

		public override bool Delete(DataContext dataContext)
        {
            using(DbCommand command  = dataContext.CreateCommand(SQL_DELETE_REQUESTTOKENS))
            {							
				command.Parameters.Add(dataContext.CreateParameter(this.UserName, PARAM_USERNAME));				
				command.Parameters.Add(dataContext.CreateParameter(this.TokenHash, PARAM_TOKENHASH));				
                return (command.ExecuteNonQuery() == 1);
            }
        }

        #endregion
        
        #region Entity Relationship Functions
        
        #endregion
    }
}
