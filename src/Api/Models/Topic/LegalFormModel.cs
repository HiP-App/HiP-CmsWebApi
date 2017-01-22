using System.ComponentModel.DataAnnotations;

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
