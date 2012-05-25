using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace Feedbook.Specifications.Twitter
{
    /// <summary>
    /// The class that represents a twitter user account
    /// </summary>
    [DataContract(Name="user", Namespace="")]
    [DebuggerDisplay("TwitterUser = {ScreenName}")]
    internal class TwitterUser 
    {
        /// <summary>
        /// Gets or sets the User ID.
        /// </summary>
        /// <value>The User ID.</value>
        [DataMember(Name = "id")]
        public ulong Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        [DataMember(Name = "location")]
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [DataMember(Name = "description")]
        public string Description { get; set; }
           

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        /// <value>The created date.</value>
        [DataMember(Name = "created_at")]
        public string CreatedDateString { get; set; }


        /// <summary>
        /// Gets or sets the time zone.
        /// </summary>
        /// <value>The time zone.</value>
        [DataMember(Name = "time_zone")]
        public string TimeZone { get; set; }

        /// <summary>
        /// Gets or sets the number of followers.
        /// </summary>
        /// <value>The number of followers.</value>
        [DataMember(Name = "followers_count")]
        public uint NumberOfFollowers { get; set; }

        /// <summary>
        /// Gets or sets the number of statuses.
        /// </summary>
        /// <value>The number of statuses.</value>
        [DataMember(Name = "statuses_count")]
        public uint NumberOfStatuses { get; set; }

        /// <summary>
        /// Gets or sets the number of friends.
        /// </summary>
        /// <value>The number of friends.</value>
        [DataMember(Name = "friends_count")]
        public uint NumberOfFriends { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has enabled contributors access to his or her account.
        /// </summary>
        /// <value>The is contributors enabled value.</value>
        [DataMember(Name = "contributors_enabled")]
        public bool IsContributorsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>The language.</value>
        [DataMember(Name = "lang")]
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user receives notifications.
        /// </summary>
        /// <value>
        /// <c>true</c> if the user receives notifications; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "notifications", IsRequired = false)]
        public bool? DoesReceiveNotifications { get; set; }

        /// <summary>
        /// Gets or sets the screenname.
        /// </summary>
        /// <value>The screenname.</value>
        [DataMember(Name = "screen_name")]
        public string ScreenName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the authenticated user is following this user.
        /// </summary>
        /// <value>
        /// <c>true</c> if the authenticated user is following this user; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "following", IsRequired = false)]
        public bool? IsFollowing { get; set; }

        /// <summary>
        /// Gets or sets the number of favorites.
        /// </summary>
        /// <value>The number of favorites.</value>
        [DataMember(Name = "favourites_count")]
        public uint NumberOfFavorites { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this user is protected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this user is protected; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "protected")]
        public bool IsProtected { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this user is geo enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this user is geo enabled; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "geo_enabled", IsRequired = false)]
        public bool? IsGeoEnabled { get; set; }

        /// <summary>
        /// Gets or sets the time zone offset.
        /// </summary>
        /// <value>The time zone offset.</value>
        /// <remarks>Also called the Coordinated Universal Time (UTC) offset.</remarks>
        [DataMember(Name = "utc_offset")]
        public string TimeZoneOffset { get; set; }

        /// <summary>
        /// Gets or sets the user's website.
        /// </summary>
        /// <value>The website address.</value>
        [DataMember(Name = "url")]
        public string Website { get; set; }

   
        /// <summary>
        /// Gets or sets the color of the profile background.
        /// </summary>
        /// <value>The color of the profile background.</value>
        [DataMember(Name = "profile_background_color")]
        public string ProfileBackgroundColorString { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether this user's profile background image is tiled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this user's profile background image is tiled; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "profile_background_tile", IsRequired = false)]
        public bool? IsProfileBackgroundTiled { get; set; }

        /// <summary>
        /// Gets or sets the color of the profile link.
        /// </summary>
        /// <value>The color of the profile link.</value>
        [DataMember(Name = "profile_link_color")]
        public string ProfileLinkColorString { get; set; }


        /// <summary>
        /// Gets or sets the profile background image location.
        /// </summary>
        /// <value>The profile background image location.</value>
        [DataMember(Name = "profile_background_image_url")]
        public string ProfileBackgroundImageLocation { get; set; }

        /// <summary>
        /// Gets or sets the color of the profile text.
        /// </summary>
        /// <value>The color of the profile text.</value>
        [DataMember(Name = "profile_text_color")]
        public string ProfileTextColorString { get; set; }



        /// <summary>
        /// Gets or sets the profile image location.
        /// </summary>
        /// <value>The profile image location.</value>
        [DataMember(Name = "profile_image_url")]
        public string ProfileImageLocation { get; set; }

        /// <summary>
        /// Gets or sets the color of the profile sidebar border.
        /// </summary>
        /// <value>The color of the profile sidebar border.</value>
        [DataMember(Name = "profile_sidebar_border_color")]
        public string ProfileSidebarBorderColorString { get; set; }


        // <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        [DataMember(Name = "status")]
        public TwitterStatus Status { get; set; }
    }
}
