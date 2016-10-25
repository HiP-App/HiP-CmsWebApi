using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Api.Models.Entity
{
    public class TopicUser
    {
        [Required]
        public int TopicId { get; set; }

        public virtual Topic Topic { get; set; }

        [Required]
        public int UserId { get; set; }

        public virtual User User { get; set; }

        [Required]
        public string Role { get; set; }
    }

    public class TopicUserMap
    {
        public TopicUserMap(EntityTypeBuilder<TopicUser> entityBuilder)
        {
            entityBuilder.HasKey(tu => new { tu.TopicId, tu.UserId, tu.Role });

            entityBuilder.HasOne(tu => tu.User)
                .WithMany(u => u.TopicUsers)
                .HasForeignKey(tu => tu.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            entityBuilder.HasOne(tu => tu.Topic)
                 .WithMany(t => t.TopicUsers)
                .HasForeignKey(tu => tu.TopicId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
