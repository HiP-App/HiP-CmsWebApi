using System.ComponentModel.DataAnnotations;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Models.Topic
{
    public class HtmlContentModel
    {
        [Required]
        public string HtmlContent { get; set; }
    }
}
