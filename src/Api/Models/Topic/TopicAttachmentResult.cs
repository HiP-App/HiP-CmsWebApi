using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity;
using System;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Models.Topic
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
            User = att.UserId;
            CreatedAt = att.UpdatedAt;
        }


        public int Id { get; set; }

        public string Title { get; set; }

        public Metadata Metadata { get; set; }

        public string Type { get; set; }

        public string User { get; set; } // a user ID

        public DateTime CreatedAt { get; set; }
    }
}
