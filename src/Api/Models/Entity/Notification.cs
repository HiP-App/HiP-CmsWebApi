using PaderbornUniversity.SILab.Hip.CmsApi.Models.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
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
        public int UserId { get; set; }

        public DateTime TimeStamp { get; set; }

        [Required]
        public int UpdaterId { get; set; }

        public User Updater { get; set; }

        [Required]
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

            entityBuilder.HasOne(n => n.Updater).WithMany(u => u.ProducedNotifications).HasForeignKey(n => n.UpdaterId).OnDelete(DeleteBehavior.SetNull);
            entityBuilder.HasOne(n => n.User).WithMany(u => u.Notifications).HasForeignKey(n => n.UserId);
        }
    }
}
