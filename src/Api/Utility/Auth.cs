using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Utility
{
    public static class Auth
    {
        // Adds function to get User Id from Context.User.Identity
        public static string GetUserIdentity(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            if (claimsIdentity == null) throw new InvalidOperationException("identity not found");

            var email = claimsIdentity.FindFirst("email");
            return email.Value;
        }

        public static List<Claim> GetUserRoles(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            if (claimsIdentity == null) throw new InvalidOperationException("identity not found");

            var role = claimsIdentity.FindAll("role");
            return role.ToList();
        }
    }
}
