using Api.Data;
using Api.Models.Entity;
using Api.Models.Notifications;
using Api.Models.User;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Api.Managers
{
    public class NotificationManager : BaseManager
    {
        public NotificationManager(CmsDbContext dbContext) : base(dbContext) { }

        public virtual IEnumerable<NotificationResult> GetNotificationsForTheUser(int userId, bool onlyUreard)
        {
            var query = dbContext.Notifications.Where(n => n.UserId == userId);
            if (onlyUreard)
                query = query.Where(n => !n.IsRead);

            var notifications = query.Include(n => n.Updater).Include(n => n.Topic).ToList().OrderByDescending(n => n.TimeStamp);
            List<NotificationResult> result = new List<NotificationResult>();
            foreach (Notification not in notifications)
            {
                var nr = new NotificationResult(not);
                nr.Updater = new UserResult(not.Updater);

                switch (not.Type)
                {
                    case NotificationType.TOPIC_CREATED:
                    case NotificationType.TOPIC_UPDATED:
                    case NotificationType.TOPIC_ASSIGNED_TO:
                    case NotificationType.TOPIC_REMOVED_FROM:
                        nr.Data = new object[] { not.Topic.Id, not.Topic.Title };
                        break;
                    case NotificationType.TOPIC_STATE_CHANGED:
                    case NotificationType.TOPIC_DEADLINE_CHANGED:
                    case NotificationType.TOPIC_ATTACHMENT_ADDED:
                        nr.Data = new object[] { not.Topic.Id, not.Topic.Title, not.Data };
                        break;
                    case NotificationType.TOPIC_DELETED:
                        nr.Data = new object[] { not.Data };
                        break;
                }
                result.Add(nr);
            }
            return result;
        }


        public virtual bool MarkAsRead(int notificationId)
        {
            var notification = dbContext.Notifications.Where(n => n.NotificationId == notificationId).Single();
            if (notification != null)
            {
                notification.IsRead = true;
                dbContext.Update(notification);
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        internal int GetNotificationCount(int userId)
        {
            return dbContext.Notifications.Where(n => n.UserId == userId && !n.IsRead).Count();
        }
    }
}