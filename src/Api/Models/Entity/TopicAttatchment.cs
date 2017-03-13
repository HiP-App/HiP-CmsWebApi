using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;

namespace Api.Models.Entity
{
    public class TopicAttatchment
    {
        public TopicAttatchment(AttatchmentFormModel model)
        {
            Name = model.AttatchmentName;
            Description = model.Description;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Path { get; set; }

        public string Description { get; set; }

        public Legal Legal { get; set; }

        public string Type { get; set; }

        public int TopicId { get; set; }

        public Topic Topic { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public DateTime UpdatedAt { get; set; }

        public TopicAttatchment() { }

    }

    public class TopicAttatchmentMap
    {
        public TopicAttatchmentMap(EntityTypeBuilder<TopicAttatchment> entityBuilder)
        {
            entityBuilder.Property(t => t.UpdatedAt).ValueGeneratedOnAddOrUpdate().HasDefaultValueSql("CURRENT_TIMESTAMP");

            entityBuilder.HasOne(ta => ta.Topic).WithMany(t => t.Attatchments).HasForeignKey(at => at.TopicId);
            entityBuilder.HasOne(ta => ta.User).WithMany(u => u.Attatchments).HasForeignKey(at => at.UserId).OnDelete(DeleteBehavior.SetNull);
        }
    }
}