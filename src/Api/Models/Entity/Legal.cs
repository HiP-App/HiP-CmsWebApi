using Api.Models.Topic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;

namespace Api.Models.Entity
{
    public class Legal
    {
        public virtual TopicAttatchment TopicAttatchment { get; set; }

        [Required]
        public int TopicAttatchmentId { get; set; }

        public DateTime TimeStamp { get; set; }

        public string PublishedDate { get; set; }

        public string PublicationType { get; set; }

        public string Author { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Source { get; set; }

        public Legal()
        {

        }

        public Legal(int attatchmentId, LegalFormModel legalModel)
        {
            TopicAttatchmentId = attatchmentId;
            PublishedDate = legalModel.PublishedDate;
            PublicationType = legalModel.PublicationType;
            Author = legalModel.Author;
            Name = legalModel.Name;
            Description = legalModel.Description;
            PublishedDate = legalModel.PublishedDate;
            Source = legalModel.Source;
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
