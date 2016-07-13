using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BOL.Models
{
    public class Topic
    {
        [Key]
        public int TopicId { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }
              
        [Required] 
        public DateTime CreationDate
        {
            get
            {
                return this.dateCreated.HasValue
                   ? this.dateCreated.Value
                   : DateTime.Now;
            }

            set { this.dateCreated = value; }
        }

        private DateTime? dateCreated = null;

        [Required]
        public DateTime Deadline { get; set; }

        [Required]
        public string Status { get; set; }
                
    }

    public class StudentTopic: Topic
    {
        public virtual ICollection<UserTopic> Students { get; set; }
    }

    public class SupervisorTopic : Topic
    {
        public virtual ICollection<UserTopic> Supervisors { get; set; }
    }
}
