using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models
{
    public class TopicAttatchment
    {
       // public TopicAttatchment() { }

        public TopicAttatchment(AttatchmentFormModel model)
        {
            this.Name = model.Name;
            this.Description = model.Description;
        }

        [Key]
        public int Id { get; set; }

        public String Name { get; set; }

        public String Path { get; set; }

        public String Description { get; set; }

        public String Type { get; set; }

        [Required]
        public int TopicId { get; set; }

        [Required]
        public int AttatchmentUser { get; set; }

        public DateTime UpdatedAt { get; set; }


    }

    public class TopicAttatchmentMap
    {
        public TopicAttatchmentMap(EntityTypeBuilder<TopicAttatchment> entityBuilder)
        {
            entityBuilder.Property(t => t.AttatchmentUser)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}