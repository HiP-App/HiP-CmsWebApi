using System;
using System.Threading.Tasks;
using Api.Clients;
using Api.Clients.Models;
using Api.Models.Entity;
using Api.Utility;

namespace Api.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailClient _emailClient;

        public EmailSender(AppConfig appConfig)
        {
            _emailClient = new EmailClient(new Uri(appConfig.EmailService));
        }

        public Task InviteAsync(string email)
        {

            var invitationModel = new InvitationModel()
            {
                Recipient = email,
                Subject = "History in Paderborn App Einladung"
            };

            return _emailClient.EmailInvitationPostAsync(invitationModel);
        }

        public Task NotifyAsync(string email, Notification notification)
        {
            var notificationModel = new NotificationModel()
            {
                Action = notification.TypeName,
                Date = notification.TimeStamp,
                Topic = notification.Topic.Title,
                Updater = notification.Updater.FullName
            };

            return _emailClient.EmailNotificationPostAsync(notificationModel);
        }
    }
}
