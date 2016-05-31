using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using HiP_CmsWebApi.Models;
using HiP_CmsWebApi.Data;
using Microsoft.Extensions.Options;
using HiP_CmsWebApi.Models.AccountViewModels;

namespace HiP_CmsWebApi.Migrations
{
    public class IdentityRolesDbOperations
    {
        // Using RoleManagers to Create Roles.
        public async Task CreateRoles(ApplicationDbContext context, IServiceProvider serviceProvider, IOptions<LoginViewModel> userCredentials)
        {

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            // Variable to Create New role. 
            ApplicationRole applicationRole;

            // Adding these roles to the DbContext.
            foreach (var role in Constants.Roles.AllRoles)
            {
                applicationRole = new ApplicationRole { Name = role , NormalizedName = role.ToUpper()};
                var roleExit = await roleManager.RoleExistsAsync(applicationRole.NormalizedName);
                if (!roleExit)
                {
                    context.Roles.Add(applicationRole);
                    context.SaveChanges();
                }
            }

            // Invoking method to create a default admin user.
            await CreateUser(context, userManager, userCredentials);
        }

        // Create Superuser Admin User.
        public async Task CreateUser(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IOptions<LoginViewModel> userCredentials)
        {
            // Checking if the user exixts.
            var adminUser = await userManager.FindByEmailAsync(userCredentials.Value.Email);

            if (adminUser == null)
            {
                // Creating a new user and giving the user the Superuser role.
                adminUser = new ApplicationUser()
                {
                    UserName = userCredentials.Value.Email,
                    Email = userCredentials.Value.Email,
                };

                await userManager.CreateAsync(adminUser, userCredentials.Value.Password);
            }

            // Assigning Superuser role if user admin already exists.
            if (!(await userManager.IsInRoleAsync(adminUser, Constants.Roles.Admin)))
                await userManager.AddToRoleAsync(adminUser, Constants.Roles.Admin);
        }
    }
}
