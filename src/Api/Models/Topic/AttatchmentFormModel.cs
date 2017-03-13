using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class AttatchmentFormModel
    {
        [Required]
        public string AttatchmentName { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
