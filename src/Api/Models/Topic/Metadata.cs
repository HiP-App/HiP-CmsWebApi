using System;
using Api.Models.Entity;

namespace Api.Models.Topic
{
    public class Metadata
    {

        public Metadata(TopicAttachmentMetadata metadata)
        {
            Details = metadata.Details;
            Type = metadata.Type;
            SubType = metadata.SubType;
            Photographer = metadata.Photographer;
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
            TimeStamp = metadata.TimeStamp;
            DetailedPosition = metadata.DetailedPosition;
        }
        public Metadata() { }

        public string Details { get; set; }
        public string Type { get; set; }
        public string SubType { get; set; }
        public string Photographer { get; set; }
    
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
    }
}
