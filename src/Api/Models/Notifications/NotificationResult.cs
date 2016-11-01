using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models.Entity;

namespace Api.Models.Notifications
{
    public class NotificationResult
    {

        public NotificationResult(Notification not)
        {
            NotificationId = not.NotificationId;
            TimeStamp = not.TimeStamp;
            Type = not.Type.ToString();
            IsRead = not.IsRead;
        }

        public int NotificationId { get; set; }

        public DateTime TimeStamp { get; set; }

        public String Updater { get; set; }

        public string Type { get; set; }

        public object[] Data { get; set; }

        public bool IsRead { get; set; }
    }
}
