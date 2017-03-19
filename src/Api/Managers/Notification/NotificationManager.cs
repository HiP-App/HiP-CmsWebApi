using Api.Data;
using Api.Models.Entity;
using Api.Models.Notifications;
using Api.Models.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Api.Managers
{
    public class NotificationManager : BaseManager
    {
        public NotificationManager(CmsDbContext dbContext) : base(dbContext) { }

        public IEnumerable<NotificationResult> GetNotificationsForTheUser(string userIdentity, bool onlyUnread)
        {
            var userId = GetUserByIdentity(userIdentity).Id;
            var query = DbContext.Notifications.Where(n => n.UserId == userId);
            if (onlyUnread)
                query = query.Where(n => !n.IsRead);

            var notifications = query.Include(n => n.Updater).Include(n => n.Topic).ToList().OrderByDescending(n => n.TimeStamp);
            var result = new List<NotificationResult>();
            foreach (Notification not in notifications)
            {
                var nr = new NotificationResult(not) {Updater = new UserResult(not.Updater)};

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

        internal int GetNotificationCount(string userIdentity)
        {
            var userId = GetUserByIdentity(userIdentity).Id;
            return DbContext.Notifications.Count(n => n.UserId == userId && !n.IsRead);
        }

        public bool SetSubscription(string userIdentity, NotificationType type, bool subscribe)
        {
            var userId = GetUserByIdentity(userIdentity).Id;
            User user = DbContext.Users.Single(u => u.Id == userId);
            Subscription sub = new Subscription
            {
                Subscriber = user,
                Type = type
            };
            return subscribe ? AddSubscription(sub) : RemoveSubscription(sub);
        }

        public IEnumerable<string> GetSubscriptions(string userIdentity)
        {
            var userId = GetUserByIdentity(userIdentity).Id;
            return DbContext.Subscriptions.Where(
                subscription => subscription.SubscriberId == userId
            ).ToList().Select(
                subscription => subscription.TypeName
            );
        }

        private bool AddSubscription(Subscription sub)
        {
            try
            {
                var subs = FindSubscriptionsLike(sub);
                if (subs.Any())
                {
                    DbContext.Subscriptions.Update(subs.First());
                } else
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
            return DbContext.Subscriptions.Where(
                candidate => sub.Subscriber == candidate.Subscriber && sub.TypeName == candidate.TypeName
            );
        }
    }
}