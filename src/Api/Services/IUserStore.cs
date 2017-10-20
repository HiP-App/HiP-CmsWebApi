using PaderbornUniversity.SILab.Hip.CmsApi.Models.User;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Services
{
    public interface IUserStore
    {
        [Get("/api/User/{userId}")]
        Task<UserResult> GetUserAsync(string userId);

        [Get("/api/User/Me")]
        Task<UserResult> GetCurrentUserAsync();

        [Get("/api/User")]
        Task<IReadOnlyList<UserResult>> GetAllUsersAsync();
    }
}
