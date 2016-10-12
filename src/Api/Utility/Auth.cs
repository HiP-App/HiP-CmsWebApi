using Api.Data;
using BLL.Managers;
using BOL.Data;
using BOL.Models;
using Microsoft.EntityFrameworkCore;
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
    }
}
