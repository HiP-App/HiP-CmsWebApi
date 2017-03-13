using Api.Models.Entity;
using Api.Models.User;
using System;

namespace Api.Models.Topic
{
    public class DocumentResult
    {

        public DocumentResult(Document document)
        {
            Content = document.Content;
            TimeStamp = document.TimeStamp;
            if (document.Updater != null)
                Updater = new UserResult(document.Updater);
        }

        public DateTime TimeStamp { get; set; }

        public UserResult Updater { get; set; }

        public string Content { get; set; }
    }
}
