using Api.Data;
using BLL.Managers;
using BOL.Models;
using System.Security.Claims;
using System.Security.Principal;

namespace Api.Utility
{
    public static class Auth
    {
        // Adds function to get User Id from Context.User.Identity
        public static int GetUserId(this IIdentity identity)
        {
            return int.Parse((identity as ClaimsIdentity).FindFirst("Id").Value);
        }

        public static void OnTokenValidationSuccess(ClaimsIdentity identity, ApplicationDbContext dbContext)
        {
            var userManager = new UserManager(dbContext);

            var user = userManager.GetUserByEmailAsync(identity.Name).Result;

            if (user == null)
            {
                user = new Student { Email = identity.Name };
                userManager.AddUserAsync(user);
            }

            // Adding Claims for the current request user.
            identity.AddClaim(new Claim("Id", user.Id.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            identity.AddClaim(new Claim(ClaimTypes.Role, user.Role));
        }
    }
}
