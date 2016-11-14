using Api.Data;
using Api.Managers;
using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var user = userManager.GetUserById(userId);
            // Admin or Supervisor?
            return (user.Role.Equals(Role.Administrator) || user.Role.Equals(Role.Supervisor));
        }
        
        public bool IsAllowedToCreateTags(int userId)
        {
            var user = userManager.GetUserById(userId);
            return user.Role.Equals(Role.Administrator) || user.Role.Equals(Role.Supervisor);
        }

    }
}
