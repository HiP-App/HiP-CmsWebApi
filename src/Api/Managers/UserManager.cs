using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using PaderbornUniversity.SILab.Hip.CmsApi.Utility;
using PaderbornUniversity.SILab.Hip.UserStore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IReadOnlyCollection<UserResult>> GetAllUsersAsync()
        {
            return await _usersClient.GetAllAsync();
        }

        public async Task<PagedResult<UserResult>> GetAllUsersAsync(string query, string role, int page, int pageSize)
        {
            var users = (await GetAllUsersAsync())
                .Where(u => string.IsNullOrEmpty(query) || u.Email.Contains(query) || u.FirstName.Contains(query) || u.LastName.Contains(query))
                .Where(u => string.IsNullOrEmpty(role) || u.Roles.Contains(role))
                .ToList();

            return (page == 0)
                ? new PagedResult<UserResult>(users, users.Count)
                : new PagedResult<UserResult>(users.Skip((page - 1) * pageSize).Take(pageSize).ToList(), page, pageSize, users.Count);
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