using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BOL.Models
{
    public class TopicUser
    {
        [Required]
        public int TopicId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string Role { get; set; }
    }

    public class TopicUserMap
    {
        public TopicUserMap(EntityTypeBuilder<TopicUser> entityBuilder)
        {
            entityBuilder.HasKey(tu => new { tu.TopicId, tu.UserId, tu.Role });
        }
    }
}
