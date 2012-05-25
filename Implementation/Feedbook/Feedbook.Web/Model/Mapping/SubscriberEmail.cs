/*
 * Author: CrystalMapper 
 * 
 * Date:  Monday, March 14, 2011 3:54 AM
 * 
 * Class: SubscriberEmail
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

namespace VI.TalentExperts.Model
{
	[Table(TABLE_NAME)]
    public partial class SubscriberEmail : Entity< SubscriberEmail>  
    {		
		#region Table Schema
		
        public const string TABLE_NAME = "dbo.SubscriberEmails";	
     
		public const string COL_EMAILID = "EmailID";
		public const string COL_EMAIL = "Email";
		public const string COL_SUBSCRIBERID = "SubscriberID";
		public const string COL_CREATEDON = "CreatedOn";
		public const string COL_CREATEDBY = "CreatedBy";
		
        public const string PARAM_EMAILID = "@EmailID";	
        public const string PARAM_EMAIL = "@Email";	
        public const string PARAM_SUBSCRIBERID = "@SubscriberID";	
        public const string PARAM_CREATEDON = "@CreatedOn";	
        public const string PARAM_CREATEDBY = "@CreatedBy";	
		
        #endregion
		
		#region Queries
		
		private const string SQL_INSERT_SUBSCRIBEREMAILS = "INSERT INTO dbo.SubscriberEmails( [Email], [SubscriberID], [CreatedOn], [CreatedBy]) VALUES ( @Email, @SubscriberID, @CreatedOn, @CreatedBy);"   + " SELECT SCOPE_IDENTITY();" ;
		
		private const string SQL_UPDATE_SUBSCRIBEREMAILS = "UPDATE dbo.SubscriberEmails SET  [Email] = @Email, [SubscriberID] = @SubscriberID, [CreatedOn] = @CreatedOn, [CreatedBy] = @CreatedBy WHERE [EmailID] = @EmailID";
		
		private const string SQL_DELETE_SUBSCRIBEREMAILS = "DELETE FROM dbo.SubscriberEmails WHERE  [EmailID] = @EmailID ";
		
        #endregion
        	  	
        #region Declarations
        
		protected long emailid = default(long);
	
		protected string email = default(string);
	
		protected long subscriberid = default(long);
	
		protected System.DateTime createdon = default(System.DateTime);
	
		protected string createdby = default(string);
	
		protected Subscriber subscriberRef;
	
        #endregion

 		#region Properties	

        [Column( COL_EMAILID, PARAM_EMAILID, default(long))]
                              public virtual long EmailID 
        {
            get { return this.emailid; }
			set	{ 
                  if(this.emailid != value)
                    {
                        this.OnPropertyChanging(new PropertyChangingEventArgs("EmailID"));  
                        this.emailid = value;                        
                        this.OnPropertyChanged(new PropertyChangedEventArgs("EmailID"));
                    }   
                }
        }	
		
        [Column( COL_EMAIL, PARAM_EMAIL )]
                              public virtual string Email 
        {
            get { return this.email; }
			set	{ 
                  if(this.email != value)
                    {
                        this.OnPropertyChanging(new PropertyChangingEventArgs("Email"));  
                        this.email = value;                        
                        this.OnPropertyChanged(new PropertyChangedEventArgs("Email"));
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
		
        [Column( COL_SUBSCRIBERID, PARAM_SUBSCRIBERID, default(long))]
                              public virtual long SubscriberID                
        {
            get
            {
                if(this.subscriberRef == null)
                    return this.subscriberid ;
                
                return this.subscriberRef.SubscriberID;            
            }
            set
            {
                if(this.subscriberid != value)
                {
                    this.OnPropertyChanging(new PropertyChangingEventArgs("SubscriberID"));                    
                    this.subscriberid = value;                    
                    this.OnPropertyChanged(new PropertyChangedEventArgs("SubscriberID"));
                    
                    this.subscriberRef = null;
                }                
            }          
        }	
        
        public Subscriber SubscriberRef
        {
            get { 
                    if(this.subscriberRef == null
                       && this.subscriberid != default(long)) 
                    {
                        Subscriber subscriberQuery = new Subscriber {
                                                        SubscriberID = this.subscriberid  
                                                        };
                        
                        Subscriber[]  subscribers = subscriberQuery.ToList();                        
                        if(subscribers.Length == 1)
                            this.subscriberRef = subscribers[0];                        
                    }
                    
                    return this.subscriberRef; 
                }
			set	{ 
                  if(this.subscriberRef != value)
                    {
                        this.OnPropertyChanging(new PropertyChangingEventArgs("SubscriberRef"));
                        if (this.subscriberRef != null)
                            this.Parents.Remove(this.subscriberRef);                            
                        
                        if((this.subscriberRef = value) != null) 
                        {
                            this.Parents.Add(this.subscriberRef); 
                            this.subscriberid = this.subscriberRef.SubscriberID;
                        }
                        else
                        {
		                    this.subscriberid = default(long);
                        }
                        this.OnPropertyChanged(new PropertyChangedEventArgs("SubscriberRef"));
                    }   
                }
        }	
		
        
        #endregion        
        
        #region Methods     
		
       public SubscriberEmail()
        {
        }
        
        public override bool Equals(object obj)
        {
            SubscriberEmail entity = obj as SubscriberEmail;           
            
            return (
                    object.ReferenceEquals(this, entity)                    
                    || (
                        entity != null            
                        && this.EmailID == entity.EmailID
                        && this.EmailID != default(long)
                        )
                    );           
        }
        
        public override int GetHashCode()
        {
            
            int hashCode = 7;
            
            hashCode = (11 * hashCode) + this.emailid.GetHashCode();
                        
            return hashCode;          
        }
        
		public override void Read(DbDataReader reader)
		{       
			this.emailid = (long)reader[COL_EMAILID];
			this.email = (string)reader[COL_EMAIL];
			this.subscriberid = (long)reader[COL_SUBSCRIBERID];
			this.createdon = (System.DateTime)reader[COL_CREATEDON];
			this.createdby = (string)reader[COL_CREATEDBY];
            base.Read(reader);
		}
		
		public override bool Create(DataContext dataContext)
        {
            using(DbCommand command  = dataContext.CreateCommand(SQL_INSERT_SUBSCRIBEREMAILS))
            {	
				command.Parameters.Add(dataContext.CreateParameter(this.Email, PARAM_EMAIL));
				command.Parameters.Add(dataContext.CreateParameter(this.SubscriberID, PARAM_SUBSCRIBERID));
				command.Parameters.Add(dataContext.CreateParameter(this.CreatedOn, PARAM_CREATEDON));
				command.Parameters.Add(dataContext.CreateParameter(this.CreatedBy, PARAM_CREATEDBY));
                this.EmailID = Convert.ToInt64(command.ExecuteScalar());
                return true;                
            }
        }

		public override bool Update(DataContext dataContext)
        {
            using(DbCommand command  = dataContext.CreateCommand(SQL_UPDATE_SUBSCRIBEREMAILS))
            {							
				command.Parameters.Add(dataContext.CreateParameter(this.EmailID, PARAM_EMAILID));
				command.Parameters.Add(dataContext.CreateParameter(this.Email, PARAM_EMAIL));
				command.Parameters.Add(dataContext.CreateParameter(this.SubscriberID, PARAM_SUBSCRIBERID));
				command.Parameters.Add(dataContext.CreateParameter(this.CreatedOn, PARAM_CREATEDON));
				command.Parameters.Add(dataContext.CreateParameter(this.CreatedBy, PARAM_CREATEDBY));
			
                return (command.ExecuteNonQuery() == 1);
            }
        }

		public override bool Delete(DataContext dataContext)
        {
            using(DbCommand command  = dataContext.CreateCommand(SQL_DELETE_SUBSCRIBEREMAILS))
            {							
				command.Parameters.Add(dataContext.CreateParameter(this.EmailID, PARAM_EMAILID));				
                return (command.ExecuteNonQuery() == 1);
            }
        }

        #endregion
        
        #region Entity Relationship Functions
        
        #endregion
    }
}
