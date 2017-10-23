using PaderbornUniversity.SILab.Hip.CmsApi.Models.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        [Required]
        public string UserId { get; set; }

        public DateTime TimeStamp { get; set; }

        [Required]
        public string UpdaterId { get; set; } // a user ID

        [Required]
        [NotMapped]
        // Store as String to avoid inconsistency
        public NotificationType Type
        {
            get => Enum.TryParse(TypeName, out NotificationType type) ? type : NotificationType.UNKNOWN;
            set => TypeName = value.ToString();
        }

        public string TypeName { get; set; }

        public string Data { get; set; }

        // At first for Topic, should be generalzied later.
        [Required]
        public int TopicId { get; set; }

        public Topic Topic { get; set; }

        public bool IsRead { get; set; }

        public static void ConfigureModel(EntityTypeBuilder<Notification> entityBuilder)
        {
            entityBuilder.Property(n => n.TimeStamp).ValueGeneratedOnAddOrUpdate().HasDefaultValueSql("CURRENT_TIMESTAMP");
            entityBuilder.Property(n => n.IsRead).ValueGeneratedOnAdd().HasDefaultValueSql("false");
        }
    }
}
