using Api.Models.User;
using System;

namespace Api.Models.Topic
{
    public class TopicResult
    {

        public TopicResult(Api.Models.Entity.Topic topic)
        {
            this.Id = topic.Id;
            this.Title = topic.Title;
            this.Status = topic.Status;
            this.Deadline = topic.Deadline;
            this.Description = topic.Description;
            this.Requirements = topic.Requirements;
            this.CreatedAt = topic.CreatedAt;
            this.UpdatedAt = topic.UpdatedAt;

            if (topic.CreatedBy != null)
                this.CreatedBy = new UserResult(topic.CreatedBy);

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
