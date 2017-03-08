using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Api.Models.AnnotationTag;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Models.Entity.Annotation
{
    /// <summary>
    /// Represents a *directed* relation between two tags (i.e. AnnotationTag).
    /// If you want an undirected tag relation between A and B, add two relations A->B and B->A.
    /// </summary>
    public class AnnotationTagRelation : RelationRule
    {
        [Required]
        public int FirstTagId { get; set; }

        public virtual AnnotationTag FirstTag { get; set; }

        [Required]
        public int SecondTagId { get; set; }

        public virtual AnnotationTag SecondTag { get; set; }

        public string Name { get; set; }
        

        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")] // Not good but otherwise tests don't work.
        public AnnotationTagRelation(AnnotationTag firstTag, AnnotationTag secondTag, string name = "", string arrowStyle = "", string color = "")
        {
            FirstTag = firstTag;
            FirstTagId = firstTag.Id;
            SecondTag = secondTag;
            SecondTagId = secondTag.Id;
            Name = name;
            ArrowStyle = arrowStyle;
            Color = color;
        }

        public AnnotationTagRelation() { }

        public AnnotationTagRelation(RelationFormModel model)
        {
            FirstTagId = model.FirstTagId;
            SecondTagId = model.SecondTagId;
            Name = model.Name;
            ArrowStyle = model.ArrowStyle;
            Color = model.Color;
        }
    }

    public class TagRelationMap
    {
        public TagRelationMap(EntityTypeBuilder<AnnotationTagRelation> entityBuilder)
        {
            entityBuilder.HasKey(r => new { r.FirstTagId, r.SecondTagId });
            entityBuilder.HasOne(r => r.FirstTag).WithMany(t => t.TagRelations).HasForeignKey(r => r.FirstTagId).OnDelete(DeleteBehavior.Cascade);
            entityBuilder.HasOne(r => r.SecondTag).WithMany(t => t.IncomingRelations).HasForeignKey(r => r.SecondTagId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
