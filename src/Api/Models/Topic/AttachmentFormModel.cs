using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class AttachmentFormModel
    {
        [Required]
        public string Title { get; set; }

    }
}
