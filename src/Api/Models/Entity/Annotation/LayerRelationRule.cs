using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity.Annotation
{
    public class LayerRelationRule : RelationRule
    {
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
