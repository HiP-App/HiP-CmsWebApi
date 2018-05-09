using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using PaderbornUniversity.SILab.Hip.CmsApi.Utility;
using PaderbornUniversity.SILab.Hip.UserStore;
using System;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Managers
{
    public class UserManager
    {
        private readonly UserStoreService _userStoreService;

        public UserManager(UserStoreService service, IOptions<UserStoreConfig> userStoreConfig)
        {
            if (string.IsNullOrWhiteSpace(userStoreConfig.Value?.UserStoreHost))
                throw new ArgumentException($"The property '{nameof(UserStoreConfig)}.{nameof(UserStoreConfig.UserStoreHost)}' is not configured");
            _userStoreService = service;
        }

        public async Task<UserResult> GetUserByIdAsync(string userId)
        {
            return await _userStoreService.Users.GetByIdAsync(userId);
        }

        public async Task<UserResult> GetUserByEmailAsync(string email)
        {
            return await _userStoreService.Users.GetByEmailAsync(email);
        }
    }
}