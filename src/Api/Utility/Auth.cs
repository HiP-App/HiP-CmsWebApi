using System;
using System.Security.Claims;
using System.Security.Principal;

namespace Api.Utility
{
    public static class Auth
    {
        // Adds function to get User Id from Context.User.Identity
        public static int GetUserId(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            if (claimsIdentity != null)
                return int.Parse(claimsIdentity.FindFirst("Id").Value);
            throw new InvalidOperationException("identity not found");
        }
    }
}
