using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BOL.Models
{
    public class Topic
    {
        [Key]
        public int Id { get; set; }

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

        public string Requirements { get; set; }

        public virtual ICollection<StudentTopic> Students { get; set; }

        public virtual ICollection<SupervisorTopic> Supervisors { get; set; }

        public int ReviewerId { get; set; }
        public Supervisor Reviewer { get; set; }

        public Topic(TopicFormModel model)
        {
            Title = model.Title;
            Description = model.Description;
            Deadline = model.Deadline;
            Students = model.Students;
            Supervisors = model.Supervisors;
            Requirements = model.Requirements;
            ReviewerId = model.ReviewerId;
            Status = model.Status;
        }
    }
}
