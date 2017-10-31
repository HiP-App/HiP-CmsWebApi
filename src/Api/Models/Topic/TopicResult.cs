using System;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Models.Topic
{
    public class TopicResult
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Status { get; set; }

        public DateTime Deadline { get; set; }

        public string Description { get; set; }

        public string Requirements { get; set; }

        public string CreatedBy { get; set; } // a user ID

        public DateTime CreatedAt { get; set; }

        public string UpdatedBy { get; set; } // a user ID

        public DateTime UpdatedAt { get; set; }

        public TopicResult(Entity.Topic topic)
        {
            Id = topic.Id;
            Title = topic.Title;
            Status = topic.Status;
            Deadline = topic.Deadline;
            Description = topic.Description;
            Requirements = topic.Requirements;
            CreatedAt = topic.CreatedAt;
            UpdatedAt = topic.UpdatedAt;
            CreatedBy = topic.CreatedById;
            // TODO this.CreatedBy = topic.UpdatedBy;
        }
    }
}
