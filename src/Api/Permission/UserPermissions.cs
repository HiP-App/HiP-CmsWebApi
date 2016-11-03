using Api.Data;
using Api.Managers;
using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Permission
{
    public class UserPermissions : BaseManager
    {
        private UserManager userManager;


        public UserPermissions(CmsDbContext dbContext) : base(dbContext)
        {
            this.userManager = new UserManager(dbContext);
        }

        public bool IsAllowedToAdminister(int userId)
        {
            var user = userManager.GetUserById(userId);
            return user.Role.Equals(Role.Administrator);
        }
    }
}
