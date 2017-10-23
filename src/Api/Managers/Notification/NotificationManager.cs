using PaderbornUniversity.SILab.Hip.CmsApi.Data;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Notifications;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Managers
{
    public class NotificationManager : BaseManager
    {
        private readonly UserManager _userManager;

        public NotificationManager(CmsDbContext dbContext, UserManager userManager) : base(dbContext)
        {
            _userManager = userManager;
        }

        public IEnumerable<NotificationResult> GetNotificationsForTheUser(string userId, bool onlyUnread)
        {
            var query = DbContext.Notifications.Where(n => n.UserId == userId);
            if (onlyUnread)
                query = query.Where(n => !n.IsRead);

            var notifications = query.Include(n => n.Topic).OrderByDescending(n => n.TimeStamp);
            var result = new List<NotificationResult>();
            foreach (var notification in notifications)
            {
                var nr = new NotificationResult(notification);

                switch (notification.Type)
                {
                    case NotificationType.TOPIC_CREATED:
                    case NotificationType.TOPIC_UPDATED:
                    case NotificationType.TOPIC_ASSIGNED_TO:
                    case NotificationType.TOPIC_REMOVED_FROM:
                        nr.Data = new object[] { notification.Topic.Id, notification.Topic.Title };
                        break;
                    case NotificationType.TOPIC_STATE_CHANGED:
                    case NotificationType.TOPIC_DEADLINE_CHANGED:
                    case NotificationType.TOPIC_ATTACHMENT_ADDED:
                        nr.Data = new object[] { notification.Topic.Id, notification.Topic.Title, notification.Data };
                        break;
                    case NotificationType.TOPIC_DELETED:
                        nr.Data = new object[] { notification.Data };
                        break;
                    case NotificationType.UNKNOWN:
                        break;
                }
                result.Add(nr);
            }
            return result;
        }


        public bool MarkAsRead(int notificationId)
        {
            try
            {
                var notification = DbContext.Notifications.Single(n => n.NotificationId == notificationId);
                notification.IsRead = true;
                DbContext.Update(notification);
                DbContext.SaveChanges();
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        internal int GetNotificationCount(string userId)
        {
            return DbContext.Notifications.Count(n => n.UserId == userId && !n.IsRead);
        }

        public bool SetSubscription(string userId, NotificationType type, bool subscribe)
        {
            var sub = new Subscription
            {
                SubscriberId = userId,
                Type = type
            };

            return subscribe ? AddSubscription(sub) : RemoveSubscription(sub);
        }

        public IEnumerable<string> GetSubscriptions(string userId)
        {
            return DbContext.Subscriptions
                .Where(subscription => subscription.SubscriberId == userId)
                .ToList()
                .Select(subscription => subscription.TypeName);
        }

        private bool AddSubscription(Subscription sub)
        {
            try
            {
                var subs = FindSubscriptionsLike(sub);
                if (subs.Any())
                {
                    DbContext.Subscriptions.Update(subs.First());
                }
                else
                {
                    DbContext.Subscriptions.Add(sub);
                }
                DbContext.SaveChanges();
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        private bool RemoveSubscription(Subscription sub)
        {
            try
            {
                foreach (var subscription in FindSubscriptionsLike(sub))
                {
                    DbContext.Subscriptions.Remove(subscription);
                }
                DbContext.SaveChanges();
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        private IQueryable<Subscription> FindSubscriptionsLike(Subscription sub)
        {
            return DbContext.Subscriptions
                .Where(candidate => sub.SubscriberId == candidate.SubscriberId && sub.TypeName == candidate.TypeName);
        }
    }
}