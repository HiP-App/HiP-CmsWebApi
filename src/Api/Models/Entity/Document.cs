using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.Entity
{
    public class Document
    {
        // TODO KEY = TopicId
        [Key]
        public int Id { get; set; }

        [Required]
        public int TopicId { get; set; }

        public Topic Topic { get; set; }

        public DateTime TimeStamp { get; set; }

        [Required]
        public int UpdaterId { get; set; }

        public User Updater { get; set; }

        public String Content { get; set; }

        public class DocumentMap
        {
            public DocumentMap(EntityTypeBuilder<Document> entityBuilder)
            {
                entityBuilder.Property(d => d.TimeStamp).ValueGeneratedOnAddOrUpdate().HasDefaultValueSql("CURRENT_TIMESTAMP");
                entityBuilder.HasOne(d => d.Updater).WithMany(u => u.Documents).HasForeignKey(n => n.UpdaterId).OnDelete(DeleteBehavior.SetNull);
                entityBuilder.HasOne(d => d.Topic).WithMany(t => t.Documents).HasForeignKey(n => n.TopicId).OnDelete(DeleteBehavior.SetNull);

            }
        }

    }
}
