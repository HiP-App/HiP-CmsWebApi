using BLL.Managers;
using BOL.Data;
using BOL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Managers
{
    public class NotificationManager : BaseManager
    {
        public NotificationManager(CmsDbContext dbContext) : base(dbContext) { }

        public virtual IEnumerable<Notification> CurrentUser(User userId)
        {
            return (from n in dbContext.Notifications
                    where n.Id == userId.Id
                    select n).ToList();
        }

        public bool UpdateNotification(int userId, int topicId, TopicFormModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    if (model.Supervisors != null)
                    {
                        foreach (int supervisor in model.Supervisors)
                        {
                            dbContext.Notifications.Add(new Notification(new NotificationFormModel()) { Id = supervisor, ChangedById = userId, TopicId = topicId, Message = "Notification received", IsReadOrNot = false });
                        }
                    }
                    else if (model.Students != null)
                    {
                        foreach (int student in model.Students)
                        {
                            dbContext.Notifications.Add(new Notification(new NotificationFormModel()) { Id = student, ChangedById = userId, TopicId = topicId, Message = "Notification received", IsReadOrNot = false });
                        }
                    }
                    else
                    {
                        foreach (int reviewer in model.Reviewers)
                        {
                            dbContext.Notifications.Add(new Notification(new NotificationFormModel()) { Id = reviewer, ChangedById = userId, TopicId = topicId, Message = "Notification received", IsReadOrNot = false });
                        }
                    }

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
    }

}
