using System.ComponentModel.DataAnnotations;

namespace Api.Models.User
{
    public class UsersFormModel
    {
        [Required]
        public int[] Users { get; set; }
    }
}
