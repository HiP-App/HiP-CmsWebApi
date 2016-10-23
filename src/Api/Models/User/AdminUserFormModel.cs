using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class AdminUserFormModel : UserFormModel
    {
        [Required]
        public string Role { get; set; }
    }
}
