using BLL.Managers;
using BOL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Api.Data
{
    public class StartupTasks
    {
        private UserManager userManager;

        public StartupTasks(ApplicationDbContext dbContext)
        {
            userManager = new UserManager(dbContext);
        }

        //Creating a User
        public void CreateUser(string username, string role)
        {
            var addUser = userManager.AddUserAsync(username, role);
        }

        //Checking if user exists
        public void CheckandCreateUser(ClaimsIdentity claimsIdentity)
        {            
            var user = userManager.GetUserByEmailAsync(claimsIdentity.Name);

            if(user != null)
            {
                //Adding Claims to the existing user.
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, user.Result.Email));
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, user.Result.FullName));
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, user.Result.Role));
            }
            else
            {
                //Adding User to the database if he does not exist.
                CreateUser(claimsIdentity.Name, Role.Student);
            }
        }

    }
}
