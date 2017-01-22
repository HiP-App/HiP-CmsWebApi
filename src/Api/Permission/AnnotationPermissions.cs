using Api.Data;
using Api.Managers;
using Api.Models;
using System;

namespace Api.Permission
{
    public class AnnotationPermissions : BaseManager
    {
        private UserManager userManager;


        public AnnotationPermissions(CmsDbContext dbContext) : base(dbContext)
        {
            this.userManager = new UserManager(dbContext);
        }


        public bool IsAllowedToEditTags(int userId)
        {
            try
            {
                // Admin or Supervisor?
                var user = userManager.GetUserById(userId);
                return (user.Role.Equals(Role.Administrator) || user.Role.Equals(Role.Supervisor));
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        public bool IsAllowedToCreateTags(int userId)
        {
            try
            {
                var user = userManager.GetUserById(userId);
                return user.Role.Equals(Role.Administrator) || user.Role.Equals(Role.Supervisor);
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

    }
}
