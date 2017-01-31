using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class InviteFormModel
    {
        [Required]
        public string[] Emails { get; set; }
    }
}