using System.ComponentModel.DataAnnotations;

namespace Api.Models.User
{
    public class UsersFormModel
    {
        [Required]
        public string[] Users { get; set; }
    }
}
