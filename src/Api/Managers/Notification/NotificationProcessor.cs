using System;
using System.Collections.Generic;
using PaderbornUniversity.SILab.Hip.CmsApi.Data;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity;
using PaderbornUniversity.SILab.Hip.CmsApi.Models;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Notifications;
using System.Linq;
using PaderbornUniversity.SILab.Hip.CmsApi.Services;
// ReSharper disable InconsistentNaming

namespace PaderbornUniversity.SILab.Hip.CmsApi.Managers
{
    public class NotificationProcessor : BaseManager
    {

        private readonly List<int> notifiedUsers = new List<int>();
        private readonly Topic topic;
        private readonly int currentUser;
        private readonly IEmailSender emailSender;

        public NotificationProcessor(
            CmsDbContext dbContext,
            Topic currentTopic,
            string identity
        ) : base(dbContext)
        {
            topic = currentTopic;
            currentUser = GetIdByIdentity(identity);
            emailSender = (EmailSender) Startup.ServiceProvider.GetService(typeof(IEmailSender)); // TODO: This is probably not such a good idea...
            // Do not notify yourself
            notifiedUsers.Add(currentUser);
        }

        public void OnNewTopic()
        {
            NotifyAll(NotificationType.TOPIC_CREATED);
            Finish();
        }

        public void OnDeleteTopic()
        {
            NotifyAll(NotificationType.TOPIC_DELETED, topic.Title);
            Finish();
        }

        public void OnStateChanged(string state)
        {
            NotifyAll(NotificationType.TOPIC_STATE_CHANGED, state);
            Finish();
        }

        public void OnAttachmetAdded(string name)
        {
            NotifyAll(NotificationType.TOPIC_ATTACHMENT_ADDED, name);
            Finish();
        }

        private void NotifyAll(NotificationType type, string data = null)
        {
            topic.TopicUsers.ForEach(tu => CreateNotification(tu, type, data));
        }

        #region OnUpdate

        public void OnUpdate(TopicFormModel changes)
        {
            // Deadline Changed
            if (changes.Deadline != topic.Deadline)
                NotifyAll(NotificationType.TOPIC_DEADLINE_CHANGED, changes.Deadline.ToString());
            else if ((changes.Status != topic.Status))
                NotifyAll(NotificationType.TOPIC_STATE_CHANGED, changes.Status);
            else
                NotifyAll(NotificationType.TOPIC_UPDATED);

            Finish();
        }

        #endregion

        #region OnUsersChanged

        public void OnUsersChanged(IEnumerable<TopicUser> newUser, IEnumerable<TopicUser> deletedUser, string role)
        {
            foreach (TopicUser user in newUser)
            {
                CreateNotification(user, NotificationType.TOPIC_ASSIGNED_TO, role);
            }
            foreach (TopicUser user in deletedUser)
            {
                CreateNotification(user, NotificationType.TOPIC_REMOVED_FROM, role);
            }

            Finish();
        }

        #endregion

        #region createNotification

        private void CreateNotification(TopicUser topicUser, NotificationType type, string data = null)
        {
            var userId = topicUser.UserId;
            if (!notifiedUsers.Contains(userId))
            {
                var not = CreateAppNotification(type, data, userId);
                CreateMailNotification(topicUser, type, userId, not);
            }
        }

        private Notification CreateAppNotification(NotificationType type, string data, int userId)
        {
            Notification not = new Notification() { UpdaterId = currentUser, Type = type, UserId = userId };
            if (topic != null)
                not.TopicId = topic.Id;
            if (data != null)
                not.Data = data;

            notifiedUsers.Add(userId);
            DbContext.Notifications.Add(not);
            return not;
        }

        private void CreateMailNotification(TopicUser topicUser, NotificationType type, int userId, Notification not)
        {
            var email = FetchUserEmail(topicUser);
            var subscribed = IsSubscribed(type, userId);
            if (email != null && subscribed)
                emailSender.NotifyAsync(email, not);
        }

        private string FetchUserEmail(TopicUser topicUser)
        {
            throw new NotImplementedException();
            //try
            //{
            //    var user = DbContext.Users.First(candidate => candidate.Id == topicUser.UserId);
            //    return user.Email;
            //}
            //catch (InvalidOperationException)
            //{
            //    return null;
            //}
        }

        private bool IsSubscribed(NotificationType type, int userId)
        {
            return DbContext.Subscriptions.Any(subscription => subscription.Subscriber.Id == userId && subscription.Type == type);
        }

        #endregion

        private void Finish()
        {
            DbContext.SaveChanges();
        }
    }
}
