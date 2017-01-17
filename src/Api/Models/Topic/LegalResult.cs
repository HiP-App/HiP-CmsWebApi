using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models.Entity;

namespace Api.Models.Topic
{
    public class LegalResult
    {
        public LegalResult(Legal legal)
        {
            this.TimeStamp = legal.TimeStamp;
            this.PublishedDate = legal.PublishedDate;
            this.PublicationType = legal.PublicationType;
            this.Author = legal.Author;
            this.Name = legal.Name;
            this.Description = legal.Description;
        }

        public DateTime TimeStamp { get; set; }

        public string PublishedDate { get; set; }

        public string PublicationType { get; set; }

        public string Author { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
