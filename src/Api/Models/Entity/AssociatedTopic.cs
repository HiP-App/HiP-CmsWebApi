using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Models.Entity
{
    public class AssociatedTopic
    {
        [Required]
        public int? ParentTopicId { get; set; }

        public Topic ParentTopic { get; set; }

        [Required]
        public int? ChildTopicId { get; set; }

        public Topic ChildTopic { get; set; }
    }

    public class AssociatedTopicMap
    {
        public AssociatedTopicMap(EntityTypeBuilder<AssociatedTopic> entityBuilder)
        {
            entityBuilder.HasKey(t => new { t.ParentTopicId, t.ChildTopicId });

            entityBuilder.HasOne(at => at.ParentTopic).WithMany(t => t.AssociatedTopics).HasForeignKey(at => at.ParentTopicId);
            entityBuilder.HasOne(at => at.ChildTopic).WithMany(t => t.ParentTopics).HasForeignKey(at => at.ChildTopicId);
        }
    }
}
