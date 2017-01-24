using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;

namespace Api.Models.Entity
{
    /// <summary>
    /// Represents a *directed* relation between two tag instances (i.e. AnnotationTagInstance).
    /// If you want an undirected tag relation between A and B, add two relations A->B and B->A.
    /// </summary>
    public class TagRelation
    {
        [Required]
        public int FirstTagId { get; set; }

        public AnnotationTagInstance FirstTag { get; set; }

        [Required]
        public int SecondTagId { get; set; }

        public AnnotationTagInstance SecondTag { get; set; }

        public String Name { get; set; }

        public TagRelation(AnnotationTagInstance firstTag, AnnotationTagInstance secondTag, string name = "")
        {
            FirstTag = firstTag;
            FirstTagId = firstTag.Id;
            SecondTag = secondTag;
            SecondTagId = secondTag.Id;
            Name = name;
        }

        public TagRelation() { }
    }

    public class TagRelationMap
    {
        public TagRelationMap(EntityTypeBuilder<TagRelation> entityBuilder)
        {
            entityBuilder.HasKey(r => new { r.FirstTagId, r.SecondTagId });
            entityBuilder.HasOne(r => r.FirstTag).WithMany(t => t.Relations).HasForeignKey(r => r.FirstTagId).OnDelete(DeleteBehavior.Cascade);
            entityBuilder.HasOne(r => r.SecondTag).WithMany(t => t.IncomingRelations).HasForeignKey(r => r.SecondTagId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
