using System;
using Api.Models.Entity;

namespace Api.Models.Topic
{
    public class LegalResult
    {
        public LegalResult(Legal legal)
        {
            TimeStamp = legal.TimeStamp;
            PublishedDate = legal.PublishedDate;
            PublicationType = legal.PublicationType;
            Author = legal.Author;
            Name = legal.Name;
            Description = legal.Description;
            Source = legal.Source;
        }

        public DateTime TimeStamp { get; set; }

        public string PublishedDate { get; set; }

        public string PublicationType { get; set; }

        public string Author { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Source { get; set; }
    }
}
