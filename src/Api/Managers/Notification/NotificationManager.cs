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
            try
            {
                var notification = dbContext.Notifications.Where(n => n.NotificationId == notificationId).Single();
                notification.IsRead = true;
                dbContext.Update(notification);
                dbContext.SaveChanges();
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        internal int GetNotificationCount(int userId)
        {
            return dbContext.Notifications.Where(n => n.UserId == userId && !n.IsRead).Count();
        }

        public bool SetSubscription(int userId, NotificationType type, bool subscribe)
        {
            User user = dbContext.Users.Single(u => u.Id == userId);
            Subscription sub = new Subscription
            {
                Subscriber = user,
                Type = type
            };
            return subscribe ? AddSubscription(user, sub) : RemoveSubscription(user, sub);
        }

        public IEnumerable<SubscriptionResult> GetSubscriptions(int userId)
        {
            return dbContext.Subscriptions.Where(
                subscription => subscription.SubscriberId == userId
            ).Select(
                subscription => new SubscriptionResult(subscription)
            );
        }

        private bool AddSubscription(User user, Subscription sub)
        {
            try
            {
                IQueryable<Subscription> subs = findSubscriptionsLike(sub);
                if (subs.Count() > 0)
                {
                    dbContext.Subscriptions.Update(subs.First());
                } else
                {
                    dbContext.Subscriptions.Add(sub);
                }
                dbContext.SaveChanges();
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        private bool RemoveSubscription(User user, Subscription sub)
        {
            try
            {
                foreach (var subscription in findSubscriptionsLike(sub))
                {
                    dbContext.Subscriptions.Remove(subscription);
                }
                dbContext.SaveChanges();
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        private IQueryable<Subscription> findSubscriptionsLike(Subscription sub)
        {
            return dbContext.Subscriptions.Where(
                candidate => sub.Subscriber == candidate.Subscriber && sub.TypeName == candidate.TypeName
            );
        }
    }
}