using Api.Data;
using Api.Models;
using Api.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Api.Managers
{
    public class NotificationManager : BaseManager
    {
        public NotificationManager(CmsDbContext dbContext) : base(dbContext) { }

        public virtual IEnumerable<Notification> GetNotificationsForTheUser(User userId)
        {
            return dbContext.Notifications.Where(n => n.UserId == userId.Id).OrderBy(n => n.TimeStamp).ToList();
        }

        public bool UpdateNotification(int changedBy, int topicId, TopicFormModel model)
        {
            try
            {                
                if (model.Supervisors != null)
                {
                    foreach (int supervisorId in model.Supervisors)
                    {
                        if(supervisorId != changedBy)
                        {
                            dbContext.Notifications.Add(new Notification() { UserId = supervisorId, ChangedByUserId = changedBy, TopicId = topicId, Message = "You have a new notification" });
                        }                        
                    }
                }
                else if (model.Students != null)
                {
                    foreach (int studentId in model.Students)
                    {
                        if(studentId != changedBy)
                        {
                            dbContext.Notifications.Add(new Notification() { UserId = studentId, ChangedByUserId = changedBy, TopicId = topicId, Message = "You have a new notification" });
                        }                        
                    }
                }
                else
                {
                    foreach (int reviewerId in model.Reviewers)
                    {
                        if (reviewerId != changedBy)
                        {
                            dbContext.Notifications.Add(new Notification() { UserId = reviewerId, ChangedByUserId = changedBy, TopicId = topicId, Message = "You have a new notification" });
                        }                        
                    }
                }

                dbContext.SaveChanges();
                return true;
            }
            catch (Exception e)

            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        public virtual bool MarkAsRead(int notificationId)
        {
            var notification = dbContext.Notifications.Where( n => n.NotificationId== notificationId).Single();

            if (notification != null)
            {
                notification.IsRead = true;
                dbContext.Update(notification);
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public virtual bool DeleteNotification(int topicId)
        {
            var notifications = dbContext.Notifications.Where(n => n.TopicId == topicId).ToList();
            
            if (notifications != null)
            {
                foreach (Notification notification in notifications)
                {
                    dbContext.Remove(notifications);
                    dbContext.SaveChanges();
                    return true;
                }
            }

            return false;
        }
    }
}
