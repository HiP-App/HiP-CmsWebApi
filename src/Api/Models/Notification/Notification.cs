using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BOL.Models
{
    public class Notification
    {
        [Key]
        public int key { get; set; }

        [Required]
        public int Id { get; set; }

        public DateTime TimeStamp { get; set; }

        [Required]
        public int ChangedById { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public int TopicId { get; set; }

        [Required]
        public bool IsReadOrNot { get; set; }

        public Notification(NotificationFormModel model)
        {
            Id = model.UserId;
            ChangedById = model.ChangedById;
            Message = model.Message;
            TopicId = model.TopicId;
            IsReadOrNot = model.IsReadOrNot;
        }

        public Notification() { }
    }

    public class NotificationMap
    {
        public NotificationMap(EntityTypeBuilder<Notification> entityBuilder)
        {
            entityBuilder.Property(t => t.TimeStamp)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");            
        }
    }

}
