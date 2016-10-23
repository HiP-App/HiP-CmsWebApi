using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class UserFormModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }     
    }
}