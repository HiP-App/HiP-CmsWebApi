using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BOL.Models
{
    public class TopicFormModel
    {
        [Required(ErrorMessage = "Title is reuired")]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Deadline is reuired")]
        public DateTime Deadline { get; set; }

        [Required(ErrorMessage = "Status is reuired")]
        public string Status { get; set; }

        public string Requirements { get; set; }

        public int ReviewerId { get; set; }

        public virtual ICollection<StudentTopic> Students { get; set; }

        public virtual ICollection<SupervisorTopic> Supervisors { get; set; }

        
    }
}
