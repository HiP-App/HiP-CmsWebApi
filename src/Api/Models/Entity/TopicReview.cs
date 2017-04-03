using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
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
        public int ReviewerId { get; set; }

        public virtual User Reviewer { get; set; }

        public string Status { get; set; }


    }

    public class TopicReviewMap
    {
        public TopicReviewMap(EntityTypeBuilder<TopicReview> entityBuilder)
        {
            entityBuilder.HasKey(rs => new { rs.TopicId, rs.ReviewerId });

            entityBuilder.Property(rs => rs.TimeStamp).ValueGeneratedOnAddOrUpdate().HasDefaultValueSql("CURRENT_TIMESTAMP");
            entityBuilder.HasOne(rs => rs.Reviewer).WithMany(u => u.Reviews).HasForeignKey(rs => rs.ReviewerId).OnDelete(DeleteBehavior.SetNull);
            entityBuilder.HasOne(rs => rs.Topic).WithMany(t => t.Reviews).HasForeignKey(rs => rs.TopicId).OnDelete(DeleteBehavior.Cascade);

        }
    }
}
