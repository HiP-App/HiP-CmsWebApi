using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Notifications;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity
{
    public class Subscription
    {
        [Key]
        public int SubscriptionId { get; set; }

        [Required]
        public string SubscriberId { get; set; } // a user ID

        [NotMapped]
        // Store as String to avoid inconsistency
        public NotificationType Type
        {
            get
            {
                if (Enum.IsDefined(typeof(NotificationType), TypeName))
                    return (NotificationType)Enum.Parse(typeof(NotificationType), TypeName);
                return NotificationType.UNKNOWN;
            }
            set
            {
                TypeName = value.ToString();
            }
        }

        public string TypeName { get; set; }

        public static void ConfigureModel(EntityTypeBuilder<Subscription> entityBuilder)
        {
        }
    }
}
