using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using app.ViewModels.UserProfile;
using Microsoft.Extensions.OptionsModel;

namespace app.Models
{
    public class IdentityDbOperations
    {
        // Using RoleManagers to Create Roles.
        public async Task CreateRoles(ApplicationDbContext context, IServiceProvider serviceProvider, IOptions<AdminCredentialsViewModel> userCredentials)
        {

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Defining list of roles. 
            List<IdentityRole> roles = new List<IdentityRole>();
            roles.Add(new IdentityRole { Name = Constants.Admin, NormalizedName = Constants.NormalizedAdmin });
            roles.Add(new IdentityRole { Name = Constants.Supervisor, NormalizedName = Constants.NormalizedSupervisor });
            roles.Add(new IdentityRole { Name = Constants.Student, NormalizedName = Constants.NormalizedStudent });

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
                if (!(await userManager.IsInRoleAsync(adminUser, Constants.Admin)))
                    await userManager.AddToRoleAsync(adminUser, Constants.Admin);
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
                await userManager.AddToRoleAsync(newAdmin, Constants.Admin);
            }
        }
    }
}
