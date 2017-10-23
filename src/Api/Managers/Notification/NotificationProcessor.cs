using System;
using System.Collections.Generic;
using PaderbornUniversity.SILab.Hip.CmsApi.Data;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity;
using PaderbornUniversity.SILab.Hip.CmsApi.Models;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Notifications;
using System.Linq;
using PaderbornUniversity.SILab.Hip.CmsApi.Services;
using System.Threading.Tasks;
// ReSharper disable InconsistentNaming

namespace PaderbornUniversity.SILab.Hip.CmsApi.Managers
{
    public class NotificationProcessor : BaseManager
    {
        private readonly List<string> _notifiedUsers = new List<string>();
        private readonly Topic _topic;
        private readonly string _currentUser;
        private readonly IEmailSender _emailSender;
        private readonly UserManager _userManager;

        public NotificationProcessor(
            CmsDbContext dbContext,
            Topic currentTopic,
            string userId,
            UserManager userManager) : base(dbContext)
        {
            _topic = currentTopic;
            _currentUser = userId;
            _emailSender = (EmailSender)Startup.ServiceProvider.GetService(typeof(IEmailSender)); // TODO: This is probably not such a good idea...
            _userManager = userManager;
            // Do not notify yourself
            _notifiedUsers.Add(_currentUser);
        }

        public void OnNewTopic()
        {
            NotifyAll(NotificationType.TOPIC_CREATED);
            Finish();
        }

        public void OnDeleteTopic()
        {
            NotifyAll(NotificationType.TOPIC_DELETED, _topic.Title);
            Finish();
        }

        public void OnStateChanged(string state)
        {
            NotifyAll(NotificationType.TOPIC_STATE_CHANGED, state);
            Finish();
        }

        public void OnAttachmentAdded(string name)
        {
            NotifyAll(NotificationType.TOPIC_ATTACHMENT_ADDED, name);
            Finish();
        }

        private void NotifyAll(NotificationType type, string data = null)
        {
            _topic.TopicUsers.ForEach(tu => CreateNotificationAsync(tu, type, data));
        }

        #region OnUpdate

        public void OnUpdate(TopicFormModel changes)
        {
            // Deadline Changed
            if (changes.Deadline != _topic.Deadline)
                NotifyAll(NotificationType.TOPIC_DEADLINE_CHANGED, changes.Deadline.ToString());
            else if ((changes.Status != _topic.Status))
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
                CreateNotificationAsync(user, NotificationType.TOPIC_ASSIGNED_TO, role);
            }
            foreach (TopicUser user in deletedUser)
            {
                CreateNotificationAsync(user, NotificationType.TOPIC_REMOVED_FROM, role);
            }

            Finish();
        }

        #endregion

        #region createNotification

        private async Task CreateNotificationAsync(TopicUser topicUser, NotificationType type, string data = null)
        {
            var userId = topicUser.UserId;
            if (!_notifiedUsers.Contains(userId))
            {
                var not = CreateAppNotification(type, data, userId);
                await CreateMailNotificationAsync(topicUser, type, userId, not);
            }
        }

        private Notification CreateAppNotification(NotificationType type, string data, string userId)
        {
            Notification not = new Notification() { UpdaterId = _currentUser, Type = type, UserId = userId };
            if (_topic != null)
                not.TopicId = _topic.Id;
            if (data != null)
                not.Data = data;

            _notifiedUsers.Add(userId);
            DbContext.Notifications.Add(not);
            return not;
        }

        private async Task CreateMailNotificationAsync(TopicUser topicUser, NotificationType type, int userId, Notification not)
        {
            var email = await TryFetchUserEmailAsync();
            var subscribed = IsSubscribed(type, userId);

            if (email != null && subscribed)
                await _emailSender.NotifyAsync(email, not);

            async Task<string> TryFetchUserEmailAsync()
            {
                try
                {
                    var user = await _userManager.GetUserByIdAsync(topicUser.UserId);
                    return user.Email;
                }
                catch (InvalidOperationException) // TODO: Find out what UserManager throws in case of '404 Not Found'
                {
                    return null;
                }
            }
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
