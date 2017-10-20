using PaderbornUniversity.SILab.Hip.CmsApi.Models.User;
using System;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Models.Topic
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
                CreatedBy = new UserResultLegacy(topic.CreatedBy);

            // TODO this.CreatedBy = topic.UpdatedBy;
        }


        public int Id { get; set; }

        public string Title { get; set; }

        public string Status { get; set; }

        public DateTime Deadline { get; set; }

        public string Description { get; set; }

        public string Requirements { get; set; }

        public UserResultLegacy CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public UserResultLegacy UpdatedBy { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
