using PaderbornUniversity.SILab.Hip.CmsApi.Data;
using PaderbornUniversity.SILab.Hip.CmsApi.Managers;
using PaderbornUniversity.SILab.Hip.CmsApi.Models;
using System;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Permission
{
    public class AnnotationPermissions : BaseManager
    {
        private readonly UserManager _userManager;


        public AnnotationPermissions(CmsDbContext dbContext) : base(dbContext)
        {
            _userManager = new UserManager(dbContext);
        }

        private bool IsAdminOrSupervisor(string identity)
        {
            bool allowed;
            try
            {
                var user = ((BaseManager) _userManager).GetUserByEmail(identity);
                allowed = user.Role.Equals(Role.Administrator) || user.Role.Equals(Role.Supervisor);
            }
            catch (InvalidOperationException)
            {
                allowed = false;
            }
            return allowed;
        }

        public bool IsAllowedToEditTags(string identity)
        {
            return IsAdminOrSupervisor(identity);
        }

        public bool IsAllowedToCreateTags(string identity)
        {
            return IsAdminOrSupervisor(identity);
        }

        public bool IsAllowedToCreateRelationRules(string identity)
        {
            return IsAdminOrSupervisor(identity);
        }

    }
}
