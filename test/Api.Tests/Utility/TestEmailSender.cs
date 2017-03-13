using System.Threading.Tasks;
using Api.Models.Entity;
using System;
using Api.Services;

namespace Api.Tests.Utility
{
    public class TestEmailSender : IEmailSender
    {
        public Task InviteAsync(string email)
        {
            Console.WriteLine("InviteAsync " + email);
            return Task.FromResult(0);
        }

        public Task NotifyAsync(string email, Notification notification)
        {
            Console.WriteLine("NotifyAsync " + email + ": " + notification);
            return Task.FromResult(0);
        }
    }
}
