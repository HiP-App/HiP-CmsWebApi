using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity
{
    public class TopicUser
    {
        [Required]
        public int TopicId { get; set; }

        public Topic Topic { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string Role { get; set; }

        public static void ConfigureModel(EntityTypeBuilder<TopicUser> entityBuilder)
        {
            entityBuilder.HasKey(tu => new { tu.TopicId, tu.UserId, tu.Role });
            entityBuilder.HasOne(tu => tu.Topic).WithMany(t => t.TopicUsers).HasForeignKey(t => t.TopicId);
        }
    }
}
