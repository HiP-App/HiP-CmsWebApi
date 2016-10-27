using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Models.Entity
{
    public class AssociatedTopic
    {
        [Required]
        public int ParentTopicId { get; set; }

        public Topic ParentTopic { get; set; }

        [Required]
        public int ChildTopicId { get; set; }

        public Topic ChildTopic { get; set; }
    }

    public class AssociatedTopicMap
    {
        public AssociatedTopicMap(EntityTypeBuilder<AssociatedTopic> entityBuilder)
        {
            entityBuilder.HasKey(t => new { t.ParentTopicId, t.ChildTopicId });

            entityBuilder.HasOne(a => a.ChildTopic)
                .WithMany(t => t.AssociatedTopics)
                .HasForeignKey(a => a.ChildTopicId);

            entityBuilder.HasOne(a => a.ParentTopic)
             .WithMany(t => t.ParentTopics)
             .HasForeignKey(a => a.ParentTopicId);
        }
    }
}
