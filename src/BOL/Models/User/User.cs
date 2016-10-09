using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        private string ProfilePicture { get; set; }

        [NotMapped]
        public string Picture {
            get {

                if (!HasProfilePicture())
                    return "default.jpg";
                return ProfilePicture;

            }
            set { ProfilePicture = Picture; }
        }

        public bool HasProfilePicture()
        {
            return !(ProfilePicture == null || ProfilePicture.Length == 0);
        }

        public string FullName {
            get
            {
                return FirstName + ' ' + LastName;
            }
        }

        public virtual ICollection<TopicUser> TopicUsers { get; set; }
    }
}
