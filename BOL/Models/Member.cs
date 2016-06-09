using System.ComponentModel.DataAnnotations;

namespace BOL.Models
{
    public class Member
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }

        [Required]
        public string Role { get; set; }

        public string FullName {
            get
            {
                return FirstName + ' ' + LastName;
            }
        }
    }
}
