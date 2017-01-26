using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using System.IO;
using System.Linq;
using Api.Models.Entity;
using Api.Utility;

namespace Api.Services
{
    public class EmailSender : IEmailSender
    {
        private AppConfig appConfig;

        public EmailSender(AppConfig appConfig)
        {
            this.appConfig = appConfig;
        }

        private Task SendMail(string recipient, string subject, string templateFile, Dictionary<string, string> parameters) 
        {
            var smtp = appConfig.SMTPConfig;
            var bodyHtml = File.ReadAllText(Path.Combine("Services", templateFile));
            bodyHtml = parameters.Aggregate(bodyHtml, (current, entry) => current.Replace(@"{" + entry.Key + "}", entry.Value));

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("History in Paderborn", smtp.From));
            message.To.Add(new MailboxAddress("", recipient));
            message.Subject = subject;
            message.Body = new TextPart("html")
            {
                Text = bodyHtml
            };

            using (var client = new SmtpClient())
            {
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect(smtp.Server, smtp.Port, smtp.WithSSL);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                if (!string.IsNullOrEmpty(smtp.Password))
                {
                    client.Authenticate(smtp.User, smtp.Password);
                }

                client.Send(message);
                client.Disconnect(true);
            }

            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }

        public Task InviteAsync(string email)
        {
            return SendMail(
                email,
                "History in Paderborn App Einladung",
                "invation-email.html",
                new Dictionary<string, string> { { "email", email } }
            );
        }

        public Task NotifyAsync(string email, Notification notification)
        {
            var parameters = new Dictionary<string, string>
            {
                {"topic", notification.Topic.Title},
                {"updater", notification.Updater.FullName},
                {"date", notification.TimeStamp.ToString(CultureInfo.InvariantCulture)},
                {"action", notification.TypeName}
            };

            return SendMail(
                email,
                "History in Paderborn App Notification",
                "notification-email.html",
                parameters
            );
        }
    }
}
