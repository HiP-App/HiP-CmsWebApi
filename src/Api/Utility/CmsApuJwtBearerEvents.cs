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
        private readonly UserManager _userManager;

        public CmsApuJwtBearerEvents(CmsDbContext dbContext)
        {
            _userManager = new UserManager(dbContext);
        }

        public override Task AuthenticationFailed(AuthenticationFailedContext context)
        {
            return Task.FromResult(0);
        }

        public override Task TokenValidated(TokenValidatedContext context)
        {
            User user;
            var identity = context.Ticket.Principal.Identity as ClaimsIdentity;
            if (identity == null)
                return Task.FromException<ArgumentNullException>(new ArgumentNullException());

            // Just a Hack to avoid concurrency
            lock (this)
            {
                try
                {
                    user = _userManager.GetUserByEmail(identity.Name);
                }
                catch (InvalidOperationException)
                {
                    user = new User { Email = identity.Name, Role = Role.Student };
                    _userManager.AddUser(user);
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
