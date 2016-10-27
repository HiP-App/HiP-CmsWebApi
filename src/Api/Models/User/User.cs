using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BOL.Models
{
    public class User
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

        public string ProfilePicture { get; set; }

        [NotMapped]
        public string Picture
        {
            get
            {

                if (!HasProfilePicture())
                    return "default.jpg";
                return ProfilePicture;

            }
        }

        public bool HasProfilePicture()
        {
            return !(ProfilePicture == null || ProfilePicture.Length == 0);
        }

        public string FullName
        {
            get
            {
                return FirstName + ' ' + LastName;
            }
        }

        public virtual ICollection<TopicUser> TopicUsers { get; set; }


        public bool IsAdministrator()
        {
            return Role == BOL.Models.Role.Administrator;
        }

        public bool IsSupervisor()
        {
            return Role == BOL.Models.Role.Supervisor;
        }

        public bool IsStudent()
        {
            return Role == BOL.Models.Role.Student;
        }

        [ForeignKey("UserId")]
        public virtual List<Notification> Notifications { get; set; }
    }
}
