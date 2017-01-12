using Api.Models.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models.Entity
{
    /// <summary>
    /// Represents a *directed* relation between two tags.
    /// If you want an undirected tag relation between A and B, add two relations A->B and B->A.
    /// </summary>
    public class TagRelation
    {
        [Required]
        public int FirstTagId { get; set; }

        public AnnotationTag FirstTag { get; set; }

        [Required]
        public int SecondTagId { get; set; }

        public AnnotationTag SecondTag { get; set; }

        public String Name { get; set; }

        public TagRelation(AnnotationTag firstTag, AnnotationTag secondTag, string name = "")
        {
            this.FirstTag = firstTag;
            this.FirstTagId = firstTag.Id;
            this.SecondTag = secondTag;
            this.SecondTagId = secondTag.Id;
            this.Name = name;
        }

        public TagRelation() { }
    }

    public class TagRelationMap
    {
        public TagRelationMap(EntityTypeBuilder<TagRelation> entityBuilder)
        {
            entityBuilder.HasKey(r => new { r.FirstTagId, r.SecondTagId });
            entityBuilder.HasOne(r => r.FirstTag).WithMany(t => t.Relations).HasForeignKey(r => r.FirstTagId).OnDelete(DeleteBehavior.Cascade);
            entityBuilder.HasOne(r => r.SecondTag).WithMany(t => t.IncomingRelations).HasForeignKey(r => r.SecondTagId).OnDelete(DeleteBehavior.SetNull);
        }
    }
}
