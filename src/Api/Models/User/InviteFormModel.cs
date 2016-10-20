using System.ComponentModel.DataAnnotations;

namespace BOL.Models
{
    public class InviteFormModel
    {
        [Required]
        public string[] emails { get; set; }
    }
}