using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity
{
    public class TopicReview
    {
        [Required]
        public int TopicId { get; set; }

        public virtual Topic Topic { get; set; }

        public DateTime TimeStamp { get; set; }

        [Required]
        public string ReviewerId { get; set; } // a user ID

        public string Status { get; set; }

        public static void ConfigureModel(EntityTypeBuilder<TopicReview> entityBuilder)
        {
            entityBuilder.HasKey(rs => new { rs.TopicId, rs.ReviewerId });
            entityBuilder.Property(rs => rs.TimeStamp).ValueGeneratedOnAddOrUpdate().HasDefaultValueSql("CURRENT_TIMESTAMP");
            entityBuilder.HasOne(rs => rs.Topic).WithMany(t => t.Reviews).HasForeignKey(rs => rs.TopicId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
