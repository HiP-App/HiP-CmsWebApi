using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.Entity
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

        public int TopicId { get; set; }

        public virtual Topic Topic { get; set; }

        public int UserId { get; set; }

        public virtual User User { get; set; }

        public DateTime UpdatedAt { get; set; }
        public TopicAttatchment() { }

    }

    public class TopicAttatchmentMap
    {
        public TopicAttatchmentMap(EntityTypeBuilder<TopicAttatchment> entityBuilder)
        {
            entityBuilder.Property(t => t.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entityBuilder.HasOne(a => a.User)
                .WithMany(u => u.Attatchments)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            entityBuilder.HasOne(a => a.Topic)
                .WithMany(t => t.Attatchments)
                .HasForeignKey(a => a.TopicId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}