using Api.Models.User;
using System;

namespace Api.Models.Topic
{
    public class TopicResult
    {

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

            if (topic.CreatedBy != null)
                CreatedBy = new UserResult(topic.CreatedBy);

            // TODO this.CreatedBy = topic.UpdatedBy;
        }


        public int Id { get; set; }

        public string Title { get; set; }

        public string Status { get; set; }

        public DateTime Deadline { get; set; }

        public string Description { get; set; }

        public string Requirements { get; set; }

        public UserResult CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public UserResult UpdatedBy { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
