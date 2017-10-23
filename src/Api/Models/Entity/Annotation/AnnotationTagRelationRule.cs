using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity.Annotation
{
    public class AnnotationTagRelationRule : RelationRule
    {
        [Required]
        public int SourceTagId { get; set; }

        public AnnotationTag SourceTag { get; set; }

        [Required]
        public int TargetTagId { get; set; }

        public AnnotationTag TargetTag { get; set; }

        public static void ConfigureModel(EntityTypeBuilder<AnnotationTagRelationRule> entityBuilder)
        {
            entityBuilder
                .HasOne(r => r.SourceTag)
                .WithMany(l => l.TagRelationRules)
                .HasForeignKey(r => r.SourceTagId)
                .OnDelete(DeleteBehavior.Cascade);
            entityBuilder
                .HasOne(r => r.TargetTag)
                .WithMany(l => l.IncomingTagRelationRules)
                .HasForeignKey(r => r.TargetTagId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
