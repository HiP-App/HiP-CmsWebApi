using PaderbornUniversity.SILab.Hip.CmsApi.Clients;
using PaderbornUniversity.SILab.Hip.CmsApi.Clients.Models;
using PaderbornUniversity.SILab.Hip.CmsApi.Managers;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity;
using PaderbornUniversity.SILab.Hip.CmsApi.Utility;
using System;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailClient _emailClient;
        private readonly UserManager _userManager;

        public EmailSender(AppConfig appConfig, UserManager userManager)
        {
            _emailClient = new EmailClient(new Uri(appConfig.EmailService));
            _userManager = userManager;
        }

        public Task InviteAsync(string email)
        {
            var invitationModel = new InvitationModel
            {
                Recipient = email,
                Subject = "History in Paderborn App Einladung"
            };

            return _emailClient.EmailInvitationPostAsync(invitationModel);
        }

        public async Task NotifyAsync(string email, Notification notification)
        {
            var updater = await _userManager.GetUserByIdAsync(notification.UpdaterId);

            var notificationModel = new NotificationModel
            {
                Recipient = email,
                Subject = "History in Paderborn Notification",
                Action = notification.TypeName,
                Date = DateTime.Now,
                Topic = notification.Topic.Title,
                Updater = updater.FullName ?? updater.Email
            };

            await _emailClient.EmailNotificationPostAsync(notificationModel);
        }
    }
}
