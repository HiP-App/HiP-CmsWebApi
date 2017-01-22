using Api.Models.Entity;
using Api.Models.User;
using System;

namespace Api.Models.Topic
{
    public class DocumentResult
    {

        public DocumentResult(Document document)
        {
            this.Content = document.Content;
            this.TimeStamp = document.TimeStamp;
            if (document.Updater != null)
                this.Updater = new UserResult(document.Updater);
        }

        public DateTime TimeStamp { get; set; }

        public UserResult Updater { get; set; }

        public String Content { get; set; }
    }
}
