using System.ComponentModel.DataAnnotations;

namespace Api.Models.Topic
{
    public class HtmlContentModel
    {
        [Required]
        public string HtmlContent { get; set; }
    }
}
