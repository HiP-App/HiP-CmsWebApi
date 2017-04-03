using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Services
{
    public interface IEmailSender
    {
        Task InviteAsync(string email);

        Task NotifyAsync(string email, Notification notification);
    }
}
