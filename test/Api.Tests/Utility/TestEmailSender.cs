using System.Collections.Generic;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using System.IO;
using Api.Models.Entity;
using Api.Utility;
using System;

namespace Api.Tests.Utility
{
    public class TestEmailSender : IEmailSender
    {

        public TestEmailSender()
        {
        } 

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
