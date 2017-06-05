using PaderbornUniversity.SILab.Hip.CmsApi.Data;
using PaderbornUniversity.SILab.Hip.CmsApi.Managers;
using PaderbornUniversity.SILab.Hip.CmsApi.Models;
using System;
using System.Linq;
using System.Security.Principal;
using PaderbornUniversity.SILab.Hip.CmsApi.Utility;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Permission
{
    public class UserPermissions : BaseManager
    {
        private readonly UserManager _userManager;


        public UserPermissions(CmsDbContext dbContext) : base(dbContext)
        {
            _userManager = new UserManager(dbContext);
        }

        public bool IsAllowedToAdminister(IIdentity identity)
        {
            try
            {
                var roles = identity.GetUserRoles();
                return roles.Any(r => r.Value.Equals(Role.Administrator));
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        public bool IsAllowedToInvite(IIdentity identity)
        {
            try
            {
                var roles = identity.GetUserRoles();
                return roles.Any(role => role.Value.Equals(Role.Administrator) || role.Value.Equals(Role.Supervisor));
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }
    }
}
