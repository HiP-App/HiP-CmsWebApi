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

        public virtual bool IsAllowedToAdminister(int userId)
        {
            try
            {
                var user = userManager.GetUserById(userId);
                return user.Role.Equals(Role.Administrator);
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        public bool IsAllowedToInvite(int userId)
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
