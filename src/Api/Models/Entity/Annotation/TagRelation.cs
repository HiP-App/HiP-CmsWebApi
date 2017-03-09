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
        public int SourceTagId { get; set; }

        public virtual TagInstance SourceTag { get; set; }

        [Required]
        public int TargetTagId { get; set; }

        public virtual TagInstance TargetTag { get; set; }
        

        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")] // Not good but otherwise tests don't work.
        public TagRelation(TagInstance sourceTag, TagInstance targetTag, string title = "", string arrowStyle = "", string color = "")
        {
            SourceTag = sourceTag;
            SourceTagId = sourceTag.Id;
            TargetTag = targetTag;
            TargetTagId = targetTag.Id;
            Title = title;
            ArrowStyle = arrowStyle;
            Color = color;
        }

        public TagRelation() { }

        public TagRelation(RelationFormModel model)
        {
            SourceTagId = model.SourceId;
            TargetTagId = model.TargetId;
            Title = model.Title;
            ArrowStyle = model.ArrowStyle;
            Color = model.Color;
        }
    }

    public class TagRelationMap
    {
        public TagRelationMap(EntityTypeBuilder<TagRelation> entityBuilder)
        {
            entityBuilder.HasKey(r => new { FirstTagId = r.SourceTagId, SecondTagId = r.TargetTagId });
            entityBuilder.HasOne(r => r.SourceTag).WithMany(t => t.TagRelations).HasForeignKey(r => r.SourceTagId).OnDelete(DeleteBehavior.Cascade);
            entityBuilder.HasOne(r => r.TargetTag).WithMany(t => t.IncomingRelations).HasForeignKey(r => r.TargetTagId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
