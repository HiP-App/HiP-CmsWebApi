using Api.Models.Entity;
using Api.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.Topic
{
    public class TopicAttachmentResult
    {
        public TopicAttachmentResult(TopicAttatchment att)
        {
            this.Id = att.Id;
            this.Name = att.Name;
            this.Description = att.Description;
            this.Legal = new LegalResult(att.Legal);
            this.Type = att.Type;
            if (att.User != null)
                this.User = new UserResult(att.User);
            this.CreatedAt = att.UpdatedAt;
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
