using PaderbornUniversity.SILab.Hip.CmsApi.Data;
using PaderbornUniversity.SILab.Hip.CmsApi.Models;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Notifications;
using PaderbornUniversity.SILab.Hip.CmsApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task OnNewTopicAsync()
        {
            await NotifyAllAsync(NotificationType.TOPIC_CREATED);
            Finish();
        }

        public async Task OnDeleteTopicAsync()
        {
            await NotifyAllAsync(NotificationType.TOPIC_DELETED, _topic.Title);
            Finish();
        }

        public async Task OnStateChangedAsync(string state)
        {
            await NotifyAllAsync(NotificationType.TOPIC_STATE_CHANGED, state);
            Finish();
        }

        public async Task OnAttachmentAddedAsync(string name)
        {
            await NotifyAllAsync(NotificationType.TOPIC_ATTACHMENT_ADDED, name);
            Finish();
        }

        private async Task NotifyAllAsync(NotificationType type, string data = null)
        {
            foreach (var tu in _topic.TopicUsers)
                await CreateNotificationAsync(tu, type, data);
        }

        public async Task OnUpdateAsync(TopicFormModel changes)
        {
            // Deadline Changed
            if (changes.Deadline != _topic.Deadline)
                await NotifyAllAsync(NotificationType.TOPIC_DEADLINE_CHANGED, changes.Deadline.ToString());
            else if (changes.Status != _topic.Status)
                await NotifyAllAsync(NotificationType.TOPIC_STATE_CHANGED, changes.Status);
            else
                await NotifyAllAsync(NotificationType.TOPIC_UPDATED);

            Finish();
        }

        public async Task OnUsersChangedAsync(IEnumerable<TopicUser> newUser, IEnumerable<TopicUser> deletedUser, string role)
        {
            foreach (var user in newUser)
                await CreateNotificationAsync(user, NotificationType.TOPIC_ASSIGNED_TO, role);

            foreach (var user in deletedUser)
                await CreateNotificationAsync(user, NotificationType.TOPIC_REMOVED_FROM, role);

            Finish();
        }

        private async Task CreateNotificationAsync(TopicUser topicUser, NotificationType type, string data = null)
        {
            var userId = topicUser.UserId;
            if (!_notifiedUsers.Contains(userId))
            {
                var notification = CreateAppNotification(type, data, userId);
                await CreateMailNotificationAsync(topicUser, type, userId, notification);
            }
        }

        private Notification CreateAppNotification(NotificationType type, string data, string userId)
        {
            var notification = new Notification
            {
                UpdaterId = _currentUser,
                Type = type,
                UserId = userId,
                TopicId = _topic?.Id ?? 0,
                Data = data
            };

            _notifiedUsers.Add(userId);
            DbContext.Notifications.Add(notification);
            return notification;
        }

        private async Task CreateMailNotificationAsync(TopicUser topicUser, NotificationType type, string userId, Notification not)
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

        private bool IsSubscribed(NotificationType type, string userId)
        {
            return DbContext.Subscriptions.Any(subscription => subscription.SubscriberId == userId && subscription.Type == type);
        }

        private void Finish()
        {
            DbContext.SaveChanges();
        }
    }
}
