using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models.Entity
{
    public class Topic
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public DateTime Deadline { get; set; }

        public string Description { get; set; }

        public string Requirements { get; set; }

        [Required]
        public int CreatedById { get; set; }

        public virtual User CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public virtual List<TopicUser> TopicUsers { get; set; }

        public virtual List<AssociatedTopic> AssociatedTopics { get; set; }

        public virtual List<TopicAttatchment> Attatchments { get; set; }

        public Topic(TopicFormModel model)
        {
            Title = model.Title;
            Status = model.Status;
            Deadline = (DateTime)model.Deadline;
            Description = model.Description;
            Requirements = model.Requirements;

            TopicUsers = new List<TopicUser>();
            AssociatedTopics = new List<AssociatedTopic>();
        }

        public Topic() { }
    }

    public class TopicMap
    {
        public TopicMap(EntityTypeBuilder<Topic> entityBuilder)
        {
            entityBuilder.Property(t => t.CreatedAt)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entityBuilder.Property(t => t.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

        }
    }
}
