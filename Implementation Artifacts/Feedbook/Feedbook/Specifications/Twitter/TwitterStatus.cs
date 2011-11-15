using System;
using System.Globalization;
using System.Runtime.Serialization;


namespace Feedbook.Specifications.Twitter
{  
    [DataContract]
    internal class TwitterStatus 
    {     
        #region Properties
        /// <summary>
        /// Gets or sets the status id.
        /// </summary>
        /// <value>The status id.</value>
        [DataMember(Name = "id")]
        public ulong Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this status message is truncated.
        /// </summary>
        /// <value>
        /// <c>true</c> if this status message is truncated; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "truncated", IsRequired = false)]
        public bool? IsTruncated { get; set; }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        /// <value>The created date.</value>
        [DataMember(Name = "created_at")]
        public string CreatedDateString { get; set; }

        ///// <summary>
        ///// Gets the created date.
        ///// </summary>
        ///// <value>The created date.</value>
        //[IgnoreDataMember]
        //public DateTime CreatedDate
        //{
        //    get
        //    {
        //        DateTime parsedDate;

        //        if (DateTime.TryParseExact(
        //                this.CreatedDateString,
        //                DateFormat,
        //                CultureInfo.InvariantCulture,
        //                DateTimeStyles.None,
        //                out parsedDate))
        //        {
        //            return parsedDate;
        //        }
        //        else
        //        {
        //            return new DateTime();
        //        }
        //    }
        //}

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        [DataMember(Name = "source")]
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the screenName the status is in reply to.
        /// </summary>
        /// <value>The screenName.</value>
        [DataMember(Name = "in_reply_to_screen_name")]
        public string InReplyToScreenName { get; set; }

        /// <summary>
        /// Gets or sets the user id the status is in reply to.
        /// </summary>
        /// <value>The user id.</value>
        [DataMember(Name = "in_reply_to_user_id")]
        public ulong? InReplyToUserId { get; set; }

        /// <summary>
        /// Gets or sets the status id the status is in reply to.
        /// </summary>
        /// <value>The status id.</value>
        [DataMember(Name = "in_reply_to_status_id")]
        public ulong? InReplyToStatusId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the authenticated user has favorited this status.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is favorited; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "favorited", IsRequired = false)]
        public bool? IsFavorited { get; set; }

        /// <summary>
        /// Gets or sets the text of the status.
        /// </summary>
        /// <value>The status text.</value>
        [DataMember(Name = "text")]
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>The user that posted this status.</value>
        [DataMember(Name = "user")]
        public TwitterUser User { get; set; }

        /// <summary>
        /// Gets or sets the retweeted status.
        /// </summary>
        /// <value>The retweeted status.</value>
        [DataMember(Name = "retweeted_status")]
        public TwitterStatus RetweetedStatus { get; set; }
        #endregion
    }
}
