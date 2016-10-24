using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models
{
    public class TopicAttatchment
    {
        public TopicAttatchment(AttatchmentFormModel model)
        {
            this.Name = model.Name;
            this.Description = model.Description;
            this.Legal = model.Legal;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public String Name { get; set; }

        public String Path { get; set; }

        public String Description { get; set; }

        public String Legal { get; set; }

        public String Type { get; set; }

        [Required]
        [ForeignKey("TopicAttatchments")]
        public int TopicId { get; set; }

        [Required]
        public int UserId { get; set; }

        public DateTime UpdatedAt { get; set; }

        public TopicAttatchment() {}
    }

    public class TopicAttatchmentMap
    {
        public TopicAttatchmentMap(EntityTypeBuilder<TopicAttatchment> entityBuilder)
        {
            entityBuilder.Property(t => t.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}