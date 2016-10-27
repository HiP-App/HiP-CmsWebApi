using BOL.Data;
using BOL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL.Managers
{
    public class NotificationManager : BaseManager
    {
        public NotificationManager(CmsDbContext dbContext) : base(dbContext) { }

        public virtual IEnumerable<List<Notification>> CurrentUser(User userId)
        {
            return (from n in dbContext.Notifications
                    join u in dbContext.Users
                    on n.UserId equals userId.Id
                    orderby n.TimeStamp descending
                    select u.Notifications).ToList();
        }

        public bool UpdateNotification(int changedBy, int topicId, TopicFormModel model)
        {
            try
            {
                if (model.Supervisors != null)
                {
                    foreach (int supervisor in model.Supervisors)
                    {
                        dbContext.Notifications.Add(new Notification() { UserId = supervisor, ChangedByUserId = changedBy, TopicId = topicId, Message = "You are now related to a new topic " + topicId + " modified by " + changedBy});
                    }
                }
                else if (model.Students != null)
                {
                    foreach (int student in model.Students)
                    {
                        dbContext.Notifications.Add(new Notification() { UserId = student, ChangedByUserId = changedBy, TopicId = topicId, Message = "You are now related to a new topic " + topicId + " modified by " + changedBy});
                    }
                }
                else
                {
                    foreach (int reviewer in model.Reviewers)
                    {
                        dbContext.Notifications.Add(new Notification() { UserId = reviewer, ChangedByUserId = changedBy, TopicId = topicId, Message = "You are now related to a new topic " + topicId + " modified by " + changedBy});
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

        public virtual bool UpdateIsReadOrNot(int userId)
        {
            if (dbContext.Topics.AsNoTracking().FirstOrDefault(t => t.Id == userId) != null)
            {
                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        dbContext.Database.ExecuteSqlCommand($"update Notifications set \"IsRead\" = '{true}' where \"UserId\" = '{userId}'");

                        dbContext.SaveChanges();

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        transaction.Rollback();
                        return false;
                    }
                }
            }

            return false;
        }
    }
}
