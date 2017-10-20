using PaderbornUniversity.SILab.Hip.CmsApi.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// ReSharper disable CollectionNeverUpdated.Global

namespace PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity
{
    [Obsolete("Use UserResult from UserStore instead", error: false)]
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Required]
        public string Role { get; set; }

        public string ProfilePicture { get; set; }

        public List<TopicAttachment> Attachments { get; set; }

        public List<TopicUser> TopicUsers { get; set; }

        public List<Notification> Notifications { get; set; }

        public List<Notification> ProducedNotifications { get; set; }

        public List<Document> Documents { get; set; }

        public List<Subscription> Subscriptions { get; set; }

        public virtual StudentDetails StudentDetails { get; set; }

        public List<TopicReview> Reviews { get; set; }

        [NotMapped]
        public string Picture
        {
            get
            {
                if (!HasProfilePicture)
                    return Constants.DefaultPircture;
                return ProfilePicture;
            }
        }

        public bool HasProfilePicture => !string.IsNullOrEmpty(ProfilePicture);

        public string FullName
        {
            get
            {
                if (FirstName == null && LastName == null)
                    return null;
                return FirstName + ' ' + LastName;
            }
        }
    }
}
