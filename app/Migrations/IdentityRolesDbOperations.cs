using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using app.ViewModels.UserProfile;
using Microsoft.Extensions.OptionsModel;
using app.Models;

namespace app.Migrations
{
    public class IdentityRolesDbOperations
    {
        // Using RoleManagers to Create Roles.
        public async Task CreateRoles(ApplicationDbContext context, IServiceProvider serviceProvider, IOptions<AdminCredentialsViewModel> userCredentials)
        {

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Variable to Create New role. 
            IdentityRole identityRole;

            // Adding these roles to the DbContext.
            foreach (var role in Constants.Roles.AllRoles)
            {
                identityRole = new IdentityRole { Name = role , NormalizedName = role.ToUpper()};
                var roleExit = await roleManager.RoleExistsAsync(identityRole.NormalizedName);
                if (!roleExit)
                {
                    context.Roles.Add(identityRole);
                    context.SaveChanges();
                }
            }

            // Invoking method to create a default admin user.
            await CreateUser(context, userManager, userCredentials);
        }

        // Create Superuser Admin User.
        public async Task CreateUser(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IOptions<AdminCredentialsViewModel> userCredentials)
        {
            var adminUserName = userCredentials.Value.UserName;
            var adminPassword = userCredentials.Value.Password;

            // Checking if the user exixts.
            var adminUser = await userManager.FindByEmailAsync(adminUserName);

            if (adminUser != null)
            {
                // Assigning Superuser role if user admin already exists.
                if (!(await userManager.IsInRoleAsync(adminUser, Constants.Roles.Admin)))
                    await userManager.AddToRoleAsync(adminUser, Constants.Roles.Admin);
            }
            else
            {
                // Creating a new user and giving the user the Superuser role.
                var newAdmin = new ApplicationUser()
                {
                    UserName = adminUserName,
                    Email = adminUserName,
                };

                string userPWD = adminPassword;
                await userManager.CreateAsync(newAdmin, userPWD);
                await userManager.AddToRoleAsync(newAdmin, Constants.Roles.Admin);
            }
        }
    }
}
