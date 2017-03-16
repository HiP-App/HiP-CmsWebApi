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

        private bool IsAdminOrSupervisor(string userIdenty)
        {
            bool allowed;
            try
            {
                var user = _userManager.GetUserByIdenty(userIdenty);
                allowed = user.Role.Equals(Role.Administrator) || user.Role.Equals(Role.Supervisor);
            }
            catch (InvalidOperationException)
            {
                allowed = false;
            }
            return allowed;
        }

        public bool IsAllowedToEditTags(string userIdenty)
        {
            return IsAdminOrSupervisor(userIdenty);
        }

        public bool IsAllowedToCreateTags(string userIdenty)
        {
            return IsAdminOrSupervisor(userIdenty);
        }

        public bool IsAllowedToCreateRelationRules(string userIdenty)
        {
            return IsAdminOrSupervisor(userIdenty);
        }

    }
}
