using System;
using System.ComponentModel.DataAnnotations;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Topic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity
{
    public class TopicAttachmentMetadata
    {
        public virtual TopicAttachment TopicAttachment { get; set; }

        [Required]
        public int TopicAttachmentId { get; set; }

        public string Details { get; set; }
        public string Type { get; set; }
        public string SubType { get; set; }
        public string Photographer { get; set; }
        public string Creator { get; set; }
        public string Material { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int Depth { get; set; }
        public string Unit { get; set; }
        public string Date { get; set; }
        public string Date2 { get; set; }
        public string Source { get; set; }
        public string Copyright { get; set; }
        public string Location { get; set; }
        public string Signature { get; set; }
        public int Page { get; set; }
        public string PointOfOrigin { get; set; }
        public string PlaceOfManufacture { get; set; }
        public string DetailedPosition { get; set; }

        public DateTime TimeStamp { get; set; }

        public TopicAttachmentMetadata() {  }

        public TopicAttachmentMetadata(int attatchmentId, Metadata metadata)
        {
            TopicAttachmentId = attatchmentId;
            // Metadata
            Details = metadata.Details;
            Type = metadata.Type;
            SubType = metadata.SubType;
            Photographer = metadata.Photographer;
            Creator = metadata.Creator;
            Material = metadata.Material;
            Height = metadata.Height;
            Width = metadata.Width;
            Depth = metadata.Depth;
            Unit = metadata.Unit;
            Date = metadata.Date;
            Date2 = metadata.Date2;
            Source = metadata.Source;
            Copyright = metadata.Copyright;
            Location = metadata.Location;
            Signature = metadata.Signature;
            Page = metadata.Page;
            PointOfOrigin = metadata.PointOfOrigin;
            PlaceOfManufacture = metadata.PlaceOfManufacture;
            PlaceOfManufacture = metadata.PlaceOfManufacture;
            DetailedPosition = metadata.DetailedPosition;
        }

    }

    public class TopicAttachmentMetadataMap
    {
        public TopicAttachmentMetadataMap(EntityTypeBuilder<TopicAttachmentMetadata> entityBuilder)
        {
            entityBuilder.HasKey(tam => new { tam.TopicAttachmentId });

            entityBuilder.HasOne(tam => tam.TopicAttachment).WithOne(ta => ta.Metadata).OnDelete(DeleteBehavior.Cascade);
            entityBuilder.Property(tam => tam.TimeStamp).ValueGeneratedOnAddOrUpdate().HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
