using Api.Models.Entity;
using System.Threading.Tasks;

namespace Api.Utility
{
    public interface IEmailSender
    {
        Task InviteAsync(string email);

        Task NotifyAsync(string email, Notification notification);
    }
}
