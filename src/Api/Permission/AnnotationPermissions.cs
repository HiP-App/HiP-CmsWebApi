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

        private bool IsAdminOrSupervisor(int userId)
        {
            bool allowed;
            try
            {
                var user = _userManager.GetUserById(userId);
                allowed = user.Role.Equals(Role.Administrator) || user.Role.Equals(Role.Supervisor);
            }
            catch (InvalidOperationException)
            {
                allowed = false;
            }
            return allowed;
        }

        public bool IsAllowedToEditTags(int userId)
        {
            return IsAdminOrSupervisor(userId);
        }

        public bool IsAllowedToCreateTags(int userId)
        {
            return IsAdminOrSupervisor(userId);
        }

        public bool IsAllowedToCreateRelationRules(int userId)
        {
            return IsAdminOrSupervisor(userId);
        }

    }
}
