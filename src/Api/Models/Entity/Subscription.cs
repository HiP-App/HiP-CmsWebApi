using System.ComponentModel.DataAnnotations;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Notifications;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity
{
    public class Subscription
    {
        [Key]
        public int SubscriptionId { get; set; }

        [Required]
        public int SubscriberId { get; set; }

        [Required]
        public User Subscriber { get; set; }
        
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
    }

    public class SubscriptionMap
    {
        public SubscriptionMap(EntityTypeBuilder<Subscription> entityBuilder)
        {
            entityBuilder.HasOne(sub => sub.Subscriber).WithMany(user => user.Subscriptions).HasForeignKey(u => u.SubscriberId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
