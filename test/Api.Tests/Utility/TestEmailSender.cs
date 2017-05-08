﻿using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity;
using System;
using PaderbornUniversity.SILab.Hip.CmsApi.Services;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Tests.Utility
{
    public class TestEmailSender : IEmailSender //https://github.com/xunit/xunit/issues/762. It allows only with IDisposable but not with custom service.
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
