using PaderbornUniversity.SILab.Hip.CmsApi.Models.User;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Services
{
    public interface IUserStore
    {
        [Get("/api/Users/{userId}")]
        Task<UserResult> GetUserAsync(string userId);

        [Get("/api/Users/Me")]
        Task<UserResult> GetCurrentUserAsync();

        [Get("/api/Users")]
        Task<IReadOnlyList<UserResult>> GetAllUsersAsync();
    }
}
