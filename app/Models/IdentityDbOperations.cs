using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace app.Models
{
    public class IdentityDbOperations
    {
        // Using RoleManagers to Create Roles.
        public async Task CreateRoles(ApplicationDbContext context, IServiceProvider serviceProvider)
        {

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Defining list of roles. 
            List<IdentityRole> roles = new List<IdentityRole>();
            roles.Add(new IdentityRole { Name = "Admin", NormalizedName = "ADMINISTRATOR" });
            roles.Add(new IdentityRole { Name = "Supervisor", NormalizedName = "SUPERVISOR" });
            roles.Add(new IdentityRole { Name = "Student", NormalizedName = "STUDENT" });

            // Adding these roles to the DbContext.
            foreach (var role in roles)
            {
                var roleExit = await roleManager.RoleExistsAsync(role.NormalizedName);
                if (!roleExit)
                {
                    context.Roles.Add(role);
                    context.SaveChanges();
                }
            }

            // Invoking method to create a default admin user.
            await CreateUser(context, userManager);
        }

        
    }
}
