using PaderbornUniversity.SILab.Hip.CmsApi.Data;
using PaderbornUniversity.SILab.Hip.CmsApi.Managers;
using PaderbornUniversity.SILab.Hip.CmsApi.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Permission
{
    public class AnnotationPermissions : BaseManager
    {
        private readonly UserManager _userManager;

        public AnnotationPermissions(CmsDbContext dbContext, UserManager userManager) : base(dbContext)
        {
            _userManager = userManager;
        }

        private async Task<bool> IsAdminOrSupervisorAsync(string identity)
        {
            bool allowed;
            try
            {
                var user = await _userManager.GetUserByIdAsync(identity);
                allowed = user.Roles.Contains(Role.Administrator) || user.Roles.Contains(Role.Supervisor);
            }
            catch (InvalidOperationException)
            {
                allowed = false;
            }
            return allowed;
        }

        public async Task<bool> IsAllowedToEditTagsAsync(string identity)
        {
            return await IsAdminOrSupervisorAsync(identity);
        }

        public async Task<bool> IsAllowedToCreateTagsAsync(string identity)
        {
            return await IsAdminOrSupervisorAsync(identity);
        }

        public async Task<bool> IsAllowedToCreateRelationRulesAsync(string identity)
        {
            return await IsAdminOrSupervisorAsync(identity);
        }
    }
}
