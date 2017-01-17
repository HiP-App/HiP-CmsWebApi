using Api.Models.Topic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.Entity
{
    public class Legal
    {
        public virtual TopicAttatchment TopicAttatchment { get; set; }

        [Required]
        public virtual int TopicAttatchmentId { get; set; }

        public DateTime TimeStamp { get; set; }

        public string PublishedDate { get; set; }

        public string PublicationType { get; set; }

        public string Author { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
        
        public Legal()
        {

        }

        public Legal(int attatchmentId, LegalFormModel legalModel)
        {
            this.TopicAttatchmentId = attatchmentId;
            this.PublishedDate = legalModel.PublishedDate;
            this.PublicationType = legalModel.PublicationType;
            this.Author = legalModel.Author;
            this.Name = legalModel.Name;
            this.Description = legalModel.Description;
            this.PublishedDate = legalModel.PublishedDate;
        }

    }

    public class LegalMap
    {
        public LegalMap(EntityTypeBuilder<Legal> entityBuilder)
        {
            entityBuilder.HasKey(l => new { l.TopicAttatchmentId });

            entityBuilder.HasOne(l => l.TopicAttatchment).WithOne(ta => ta.Legal).OnDelete(DeleteBehavior.Cascade);
            entityBuilder.Property(l => l.TimeStamp).ValueGeneratedOnAddOrUpdate().HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
