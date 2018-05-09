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
        private readonly UsersClient _usersClient;

        public UserManager(IHttpContextAccessor context, IOptions<AppConfig> appConfig)
        {
            if (string.IsNullOrWhiteSpace(appConfig.Value?.UserStoreUrl))
                throw new ArgumentException($"The property '{nameof(AppConfig)}.{nameof(AppConfig.UserStoreUrl)}' is not configured");

            _usersClient = new UsersClient(appConfig.Value.UserStoreUrl)
            {
                Authorization = context.HttpContext.Request.Headers["Authorization"].ToString()
            };
        }
        
        public async Task<UserResult> GetUserByIdAsync(string userId)
        {
            return await _usersClient.GetByIdAsync(userId);
        }

        public async Task<UserResult> GetUserByEmailAsync(string email)
        {
            return await _usersClient.GetByIdAsync(email);
        }
    }
}