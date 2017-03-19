using Api.Data;
using Api.Managers;
using Api.Models;
using System;

namespace Api.Permission
{
    public class AnnotationPermissions : BaseManager
    {
        private readonly UserManager _userManager;


        public AnnotationPermissions(CmsDbContext dbContext) : base(dbContext)
        {
            _userManager = new UserManager(dbContext);
        }

        private bool IsAdminOrSupervisor(string userIdentity)
        {
            bool allowed;
            try
            {
                var user = _userManager.GetUserByIdentity(userIdentity);
                allowed = user.Role.Equals(Role.Administrator) || user.Role.Equals(Role.Supervisor);
            }
            catch (InvalidOperationException)
            {
                allowed = false;
            }
            return allowed;
        }

        public bool IsAllowedToEditTags(string userIdentity)
        {
            return IsAdminOrSupervisor(userIdentity);
        }

        public bool IsAllowedToCreateTags(string userIdentity)
        {
            return IsAdminOrSupervisor(userIdentity);
        }

        public bool IsAllowedToCreateRelationRules(string userIdentity)
        {
            return IsAdminOrSupervisor(userIdentity);
        }

    }
}
