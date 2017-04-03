using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity
{
    public class TopicAttachment
    {
        public TopicAttachment(AttachmentFormModel model)
        {
            Title = model.Title;
        }

        public TopicAttachment() { }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Path { get; set; }

        public TopicAttachmentMetadata Metadata { get; set; }

        public string Type { get; set; }

        public int TopicId { get; set; }

        public Topic Topic { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public DateTime UpdatedAt { get; set; }

    }

    public class TopicAttachmentMap
    {
        public TopicAttachmentMap(EntityTypeBuilder<TopicAttachment> entityBuilder)
        {
            entityBuilder.Property(t => t.UpdatedAt).ValueGeneratedOnAddOrUpdate().HasDefaultValueSql("CURRENT_TIMESTAMP");

            entityBuilder.HasOne(ta => ta.Topic).WithMany(t => t.Attachments).HasForeignKey(at => at.TopicId);
            entityBuilder.HasOne(ta => ta.User).WithMany(u => u.Attachments).HasForeignKey(at => at.UserId).OnDelete(DeleteBehavior.SetNull);
        }
    }
}
