using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity;
using System;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Models.Notifications
{
    public class NotificationResult
    {
        public NotificationResult(Notification notification)
        {
            NotificationId = notification.NotificationId;
            TimeStamp = notification.TimeStamp;
            Updater = notification.UpdaterId;
            Type = notification.Type.ToString();
            IsRead = notification.IsRead;
        }

        public int NotificationId { get; set; }

        public DateTime TimeStamp { get; set; }

        public string Updater { get; set; } // a user ID

        public string Type { get; set; }

        public object[] Data { get; set; }

        public bool IsRead { get; set; }
    }
}
