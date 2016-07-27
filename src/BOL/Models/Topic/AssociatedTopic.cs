using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BOL.Models
{
    public class AssociatedTopic
    {
        [Required]
        public int ParentTopicId { get; set; }

        [Required]
        public int ChildTopicId { get; set; }
    }

    public class AssociatedTopicMap
    {
        public AssociatedTopicMap(EntityTypeBuilder<AssociatedTopic> entityBuilder)
        {
            entityBuilder.HasKey(t => new { t.ParentTopicId, t.ChildTopicId });
        }
    }
}
