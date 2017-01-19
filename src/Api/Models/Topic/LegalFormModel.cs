using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.Topic
{
    public class LegalFormModel
    {

        public string PublishedDate { get; set; }

        public string PublicationType { get; set; }

        [Required]
        public string Author { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Source { get; set; }
    }
}
