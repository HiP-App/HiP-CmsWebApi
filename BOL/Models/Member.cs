using System.ComponentModel.DataAnnotations;

namespace BOL.Models
{
    public abstract class Member
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }

        public string FirstName { get; set; }

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
