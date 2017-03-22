using Api.Models.Entity;
using Api.Models.User;
using System;

namespace Api.Models.Topic
{
    public class TopicAttachmentResult
    {
        public TopicAttachmentResult(TopicAttachment att)
        {
            Id = att.Id;
            Title = att.Title;
            if (att.Metadata != null)
                Metadata = new Metadata(att.Metadata);
            Type = att.Type;
            if (att.User != null)
                User = new UserResult(att.User);
            CreatedAt = att.UpdatedAt;
        }


        public int Id { get; set; }

        public string Title { get; set; }

        public Metadata Metadata { get; set; }

        public string Type { get; set; }

        public UserResult User { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
