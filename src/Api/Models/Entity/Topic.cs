using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
// ReSharper disable CollectionNeverUpdated.Global

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

        // Created By
        [Required]
        public int CreatedById { get; set; }

        public User CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public List<TopicUser> TopicUsers { get; set; }

        public List<AssociatedTopic> AssociatedTopics { get; set; }

        public List<AssociatedTopic> ParentTopics { get; set; }

        public List<TopicAttatchment> Attatchments { get; set; }

        public virtual Document Document { get; set; }

        public Topic(TopicFormModel model)
        {
            Title = model.Title;
            Status = model.Status;
            if (model.Deadline != null)
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
            entityBuilder.Property(t => t.CreatedAt).ValueGeneratedOnAdd().HasDefaultValueSql("CURRENT_TIMESTAMP");
            entityBuilder.Property(t => t.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
