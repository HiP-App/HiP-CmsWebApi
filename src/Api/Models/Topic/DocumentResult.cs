using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity;
using System;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Models.Topic
{
    public class DocumentResult
    {
        public DocumentResult(Document document)
        {
            Content = document.Content;
            TimeStamp = document.TimeStamp;
            Updater = document.UpdaterId;
        }

        public DateTime TimeStamp { get; set; }

        public string Updater { get; set; } // a user ID

        public string Content { get; set; }
    }
}
