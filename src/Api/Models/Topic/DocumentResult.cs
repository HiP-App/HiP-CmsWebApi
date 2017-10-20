using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.User;
using System;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Models.Topic
{
    public class DocumentResult
    {

        public DocumentResult(Document document)
        {
            Content = document.Content;
            TimeStamp = document.TimeStamp;
            if (document.Updater != null)
                Updater = new UserResultLegacy(document.Updater);
        }

        public DateTime TimeStamp { get; set; }

        public UserResultLegacy Updater { get; set; }

        public string Content { get; set; }
    }
}
