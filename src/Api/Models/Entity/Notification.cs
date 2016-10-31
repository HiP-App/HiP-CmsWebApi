using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;

namespace Api.Models.Entity
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        [Required]
        public int UserId { get; set; }

        public DateTime TimeStamp { get; set; }

        [Required]
        public int ChangedByUserId { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public int TopicId { get; set; }

        [Required]
        public bool IsRead { get; set; }        

        public Notification() { }
    }

    public class NotificationMap
    {
        public NotificationMap(EntityTypeBuilder<Notification> entityBuilder)
        {
            entityBuilder.Property(t => t.TimeStamp)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entityBuilder.Property(t => t.IsRead)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("false");
        }
    }
}
