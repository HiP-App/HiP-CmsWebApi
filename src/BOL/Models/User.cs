using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BOL.Models
{
    public abstract class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
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

        public virtual ICollection<UserTopic> Topics { get; set; }
    }
}
