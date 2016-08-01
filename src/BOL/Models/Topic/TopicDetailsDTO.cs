using System;
using System.Collections.Generic;

namespace BOL.Models
{
    public class TopicDetailsDTO
    {
        public int Id { get; set; }
        
        public string Title { get; set; }

        public string Status { get; set; }

        public DateTime Deadline { get; set; }

        public string Description { get; set; }  

        public string Requirements { get; set; }

        public Supervisor CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public List<User> Students { get; set; }

        public List<User> Supervisors { get; set; }

        public List<User> Reviewers { get; set; }

        public List<Topic> SubTopics { get; set; }

         
    }
}