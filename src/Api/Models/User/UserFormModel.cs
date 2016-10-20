using System.ComponentModel.DataAnnotations;

namespace BOL.Models
{
    public class UserFormModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }     
    }
}