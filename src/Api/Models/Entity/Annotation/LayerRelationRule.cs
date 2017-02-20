using System.ComponentModel.DataAnnotations;
using Api.Models.AnnotationTag;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Models.Entity.Annotation
{
    public class LayerRelationRule : RelationRule
    {
        public LayerRelationRule() { }

        [Required]
        public int SourceLayerId { get; set; }

        public Layer SourceLayer { get; set; }

        [Required]
        public int TargetLayerId { get; set; }

        public Layer TargetLayer { get; set; }

        public class LayerRelationRuleMap
        {
            public LayerRelationRuleMap(EntityTypeBuilder<LayerRelationRule> entityBuilder)
            {
                entityBuilder
                    .HasOne(r => r.SourceLayer)
                    .WithMany(l => l.Relations)
                    .HasForeignKey(r => r.SourceLayerId)
                    .OnDelete(DeleteBehavior.Cascade);
                entityBuilder
                    .HasOne(r => r.TargetLayer)
                    .WithMany(l => l.IncomingRelations)
                    .HasForeignKey(r => r.TargetLayerId)
                    .OnDelete(DeleteBehavior.Cascade);
            }
        }
    }
}
