using Api.Models.Entity;
using Api.Models.User;
using System;

namespace Api.Models.Topic
{
    public class TopicAttachmentResult
    {
        public TopicAttachmentResult(TopicAttatchment att)
        {
            Id = att.Id;
            Name = att.Name;
            Description = att.Description;
            if (att.Legal != null)
                Legal = new LegalResult(att.Legal);
            Type = att.Type;
            if (att.User != null)
                User = new UserResult(att.User);
            CreatedAt = att.UpdatedAt;
        }


        public int Id { get; set; }

        public String Name { get; set; }

        public String Description { get; set; }

        public LegalResult Legal { get; set; }

        public String Type { get; set; }

        public UserResult User { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
