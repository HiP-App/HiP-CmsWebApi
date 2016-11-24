using Api.Data;
using Api.Managers;
using Api.Models;
using Api.Models.Entity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Api.Utility
{
    public class CmsApuJwtBearerEvents : JwtBearerEvents
    {
        protected readonly UserManager userManager;

        public CmsApuJwtBearerEvents(CmsDbContext dbContext)
        {
            this.userManager = new UserManager(dbContext);
        }

        public override Task AuthenticationFailed(AuthenticationFailedContext context)
        {
            return Task.FromResult(0);
        }

        public override Task TokenValidated(TokenValidatedContext context)
        {
            User user;
            var identity = context.Ticket.Principal.Identity as ClaimsIdentity;

            // Just a Hack to avoid concurrency
            lock (this)
            {
                try
                {
                    user = userManager.GetUserByEmail(identity.Name);
                }
                catch (InvalidOperationException)
                {
                    user = new User { Email = identity.Name, Role = Role.Student };
                    userManager.AddUser(user);
                }
            }

            // Adding Claims for the current request user.
            identity.AddClaim(new Claim("Id", user.Id.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            identity.AddClaim(new Claim(ClaimTypes.Role, user.Role));
            return Task.FromResult(0);
        }
    }
}
