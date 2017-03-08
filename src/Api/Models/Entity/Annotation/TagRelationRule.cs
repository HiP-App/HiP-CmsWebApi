using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Models.Entity.Annotation
{
    public class TagRelationRule: RelationRule
    {
        [Required]
        public int SourceTagId { get; set; }

        public Tag SourceTag { get; set; }

        [Required]
        public int TargetTagId { get; set; }

        public Tag TargetTag { get; set; }

        public class TagRelationRuleMap
        {
            public TagRelationRuleMap(EntityTypeBuilder<TagRelationRule> entityBuilder)
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
}
