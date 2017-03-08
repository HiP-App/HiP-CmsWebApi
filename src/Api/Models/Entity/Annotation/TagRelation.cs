using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Api.Models.AnnotationTag;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Models.Entity.Annotation
{
    /// <summary>
    /// Represents a *directed* relation between two tag instances (i.e. AnnotationTag).
    /// If you want an undirected tag relation between A and B, add two relations A->B and B->A.
    /// </summary>
    public class TagRelation : RelationRule
    {
        [Required]
        public int FirstTagId { get; set; }

        public virtual TagInstance FirstTag { get; set; }

        [Required]
        public int SecondTagId { get; set; }

        public virtual TagInstance SecondTag { get; set; }
        

        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")] // Not good but otherwise tests don't work.
        public TagRelation(TagInstance firstTag, TagInstance secondTag, string title = "", string arrowStyle = "", string color = "")
        {
            FirstTag = firstTag;
            FirstTagId = firstTag.Id;
            SecondTag = secondTag;
            SecondTagId = secondTag.Id;
            Title = title;
            ArrowStyle = arrowStyle;
            Color = color;
        }

        public TagRelation() { }

        public TagRelation(RelationFormModel model)
        {
            FirstTagId = model.SourceId;
            SecondTagId = model.TargetId;
            Title = model.Title;
            ArrowStyle = model.ArrowStyle;
            Color = model.Color;
        }
    }

    public class TagRelationMap
    {
        public TagRelationMap(EntityTypeBuilder<TagRelation> entityBuilder)
        {
            entityBuilder.HasKey(r => new { r.FirstTagId, r.SecondTagId });
            entityBuilder.HasOne(r => r.FirstTag).WithMany(t => t.TagRelations).HasForeignKey(r => r.FirstTagId).OnDelete(DeleteBehavior.Cascade);
            entityBuilder.HasOne(r => r.SecondTag).WithMany(t => t.IncomingRelations).HasForeignKey(r => r.SecondTagId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
