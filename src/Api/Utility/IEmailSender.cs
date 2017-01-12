using Api.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Utility
{
    public interface IEmailSender
    {
        Task InviteAsync(string email);

        Task NotifyAsync(string email, Notification notification);
    }
}
