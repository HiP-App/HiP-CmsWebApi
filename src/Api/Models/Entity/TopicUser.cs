using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Api.Models.Entity
{
    public class TopicUser
    {
        [Required]
        public int TopicId { get; set; }

        public Topic Topic { get; set; }

        [Required]
        public int UserId { get; set; }

        public User User { get; set; }

        [Required]
        public string Role { get; set; }
    }

    public class TopicUserMap
    {
        public TopicUserMap(EntityTypeBuilder<TopicUser> entityBuilder)
        {
            entityBuilder.HasKey(tu => new { tu.TopicId, tu.UserId, tu.Role });

            entityBuilder.HasOne(tu => tu.User).WithMany(u => u.TopicUsers).HasForeignKey(t => t.UserId);
            entityBuilder.HasOne(tu => tu.Topic).WithMany(t => t.TopicUsers).HasForeignKey(t => t.TopicId);
        }
    }
}
