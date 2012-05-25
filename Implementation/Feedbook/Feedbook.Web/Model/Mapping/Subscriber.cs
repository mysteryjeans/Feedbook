/*
 * Author: CrystalMapper 
 * 
 * Date:  Monday, March 14, 2011 3:54 AM
 * 
 * Class: Subscriber
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
    public partial class Subscriber : Entity< Subscriber>  
    {		
		#region Table Schema
		
        public const string TABLE_NAME = "dbo.Subscribers";	
     
		public const string COL_SUBSCRIBERID = "SubscriberID";
		public const string COL_UPDATEDON = "UpdatedOn";
		public const string COL_UPDATEDBY = "UpdatedBy";
		public const string COL_CREATEDON = "CreatedOn";
		public const string COL_CREATEDBY = "CreatedBy";
		
        public const string PARAM_SUBSCRIBERID = "@SubscriberID";	
        public const string PARAM_UPDATEDON = "@UpdatedOn";	
        public const string PARAM_UPDATEDBY = "@UpdatedBy";	
        public const string PARAM_CREATEDON = "@CreatedOn";	
        public const string PARAM_CREATEDBY = "@CreatedBy";	
		
        #endregion
		
		#region Queries
		
		private const string SQL_INSERT_SUBSCRIBERS = "INSERT INTO dbo.Subscribers( [UpdatedOn], [UpdatedBy], [CreatedOn], [CreatedBy]) VALUES ( @UpdatedOn, @UpdatedBy, @CreatedOn, @CreatedBy);"   + " SELECT SCOPE_IDENTITY();" ;
		
		private const string SQL_UPDATE_SUBSCRIBERS = "UPDATE dbo.Subscribers SET  [UpdatedOn] = @UpdatedOn, [UpdatedBy] = @UpdatedBy, [CreatedOn] = @CreatedOn, [CreatedBy] = @CreatedBy WHERE [SubscriberID] = @SubscriberID";
		
		private const string SQL_DELETE_SUBSCRIBERS = "DELETE FROM dbo.Subscribers WHERE  [SubscriberID] = @SubscriberID ";
		
        #endregion
        	  	
        #region Declarations
        
		protected long subscriberid = default(long);
	
		protected System.DateTime updatedon = default(System.DateTime);
	
		protected string updatedby = default(string);
	
		protected System.DateTime createdon = default(System.DateTime);
	
		protected string createdby = default(string);
	
        protected EntityCollection< SubscriberEmail> subscriberEmails ;
        
        #endregion

 		#region Properties	

        [Column( COL_SUBSCRIBERID, PARAM_SUBSCRIBERID, default(long))]
                              public virtual long SubscriberID 
        {
            get { return this.subscriberid; }
			set	{ 
                  if(this.subscriberid != value)
                    {
                        this.OnPropertyChanging(new PropertyChangingEventArgs("SubscriberID"));  
                        this.subscriberid = value;                        
                        this.OnPropertyChanged(new PropertyChangedEventArgs("SubscriberID"));
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
		
        public EntityCollection< SubscriberEmail> SubscriberEmails 
        {
            get { return this.subscriberEmails;}
        }
        
        
        #endregion        
        
        #region Methods     
		
       public Subscriber()
        {
             this.subscriberEmails = new EntityCollection< SubscriberEmail>(this, new Associate< SubscriberEmail>(this.AssociateSubscriberEmails), new DeAssociate< SubscriberEmail>(this.DeAssociateSubscriberEmails), new GetChildren< SubscriberEmail>(this.GetChildrenSubscriberEmails));
        }
        
        public override bool Equals(object obj)
        {
            Subscriber entity = obj as Subscriber;           
            
            return (
                    object.ReferenceEquals(this, entity)                    
                    || (
                        entity != null            
                        && this.SubscriberID == entity.SubscriberID
                        && this.SubscriberID != default(long)
                        )
                    );           
        }
        
        public override int GetHashCode()
        {
            
            int hashCode = 7;
            
            hashCode = (11 * hashCode) + this.subscriberid.GetHashCode();
                        
            return hashCode;          
        }
        
		public override void Read(DbDataReader reader)
		{       
			this.subscriberid = (long)reader[COL_SUBSCRIBERID];
			this.updatedon = (System.DateTime)reader[COL_UPDATEDON];
			this.updatedby = (string)reader[COL_UPDATEDBY];
			this.createdon = (System.DateTime)reader[COL_CREATEDON];
			this.createdby = (string)reader[COL_CREATEDBY];
            base.Read(reader);
		}
		
		public override bool Create(DataContext dataContext)
        {
            using(DbCommand command  = dataContext.CreateCommand(SQL_INSERT_SUBSCRIBERS))
            {	
				command.Parameters.Add(dataContext.CreateParameter(this.UpdatedOn, PARAM_UPDATEDON));
				command.Parameters.Add(dataContext.CreateParameter(this.UpdatedBy, PARAM_UPDATEDBY));
				command.Parameters.Add(dataContext.CreateParameter(this.CreatedOn, PARAM_CREATEDON));
				command.Parameters.Add(dataContext.CreateParameter(this.CreatedBy, PARAM_CREATEDBY));
                this.SubscriberID = Convert.ToInt64(command.ExecuteScalar());
                return true;                
            }
        }

		public override bool Update(DataContext dataContext)
        {
            using(DbCommand command  = dataContext.CreateCommand(SQL_UPDATE_SUBSCRIBERS))
            {							
				command.Parameters.Add(dataContext.CreateParameter(this.SubscriberID, PARAM_SUBSCRIBERID));
				command.Parameters.Add(dataContext.CreateParameter(this.UpdatedOn, PARAM_UPDATEDON));
				command.Parameters.Add(dataContext.CreateParameter(this.UpdatedBy, PARAM_UPDATEDBY));
				command.Parameters.Add(dataContext.CreateParameter(this.CreatedOn, PARAM_CREATEDON));
				command.Parameters.Add(dataContext.CreateParameter(this.CreatedBy, PARAM_CREATEDBY));
			
                return (command.ExecuteNonQuery() == 1);
            }
        }

		public override bool Delete(DataContext dataContext)
        {
            using(DbCommand command  = dataContext.CreateCommand(SQL_DELETE_SUBSCRIBERS))
            {							
				command.Parameters.Add(dataContext.CreateParameter(this.SubscriberID, PARAM_SUBSCRIBERID));				
                return (command.ExecuteNonQuery() == 1);
            }
        }

        #endregion
        
        #region Entity Relationship Functions
        
        private void AssociateSubscriberEmails(SubscriberEmail subscriberEmail)
        {
           subscriberEmail.SubscriberRef = this;
        }
        
        private void DeAssociateSubscriberEmails(SubscriberEmail subscriberEmail)
        {
          if(subscriberEmail.SubscriberRef == this)
             subscriberEmail.SubscriberRef = null;
        }
        
            
        private SubscriberEmail[] GetChildrenSubscriberEmails()
        {
            if (this.subscriberid != default(long))
            {  
                SubscriberEmail childrenQuery = new SubscriberEmail();
                childrenQuery.SubscriberRef = this;
                
                return childrenQuery.ToList(); 
            } else return null;
        }
        
        #endregion
    }
}
