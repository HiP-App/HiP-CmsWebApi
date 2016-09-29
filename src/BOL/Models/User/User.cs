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
    }
}
