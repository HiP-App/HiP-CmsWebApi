using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity.Annotation;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity
{
    public class Document
    {
        // KEY = TopicId
        [Required]
        public int TopicId { get; set; }

        public virtual Topic Topic { get; set; }

        public DateTime TimeStamp { get; set; }

        [Required]
        public int UpdaterId { get; set; }

        public User Updater { get; set; }

        [MaxLength(65536)]
        public string Content { get; set; }

        public IEnumerable<AnnotationTagInstance> TagsInstances { get; set; }

        public Document() { }

        public Document(int topicId, int userId, string htmlContent)
        {
            TopicId = topicId;
            UpdaterId = userId;
            Content = htmlContent;
        }

        // TODO tagRelations
    }
    public class DocumentMap
    {
        public DocumentMap(EntityTypeBuilder<Document> entityBuilder)
        {
            entityBuilder.HasKey(d => new { d.TopicId });

            entityBuilder.Property(d => d.TimeStamp).ValueGeneratedOnAddOrUpdate().HasDefaultValueSql("CURRENT_TIMESTAMP");
            entityBuilder.HasOne(d => d.Updater).WithMany(u => u.Documents).HasForeignKey(n => n.UpdaterId).OnDelete(DeleteBehavior.SetNull);
            entityBuilder.HasOne(d => d.Topic).WithOne(t => t.Document).OnDelete(DeleteBehavior.Cascade);
            entityBuilder.HasMany(d => d.TagsInstances).WithOne(t => t.Document).OnDelete(DeleteBehavior.Cascade);

        }
    }
}
