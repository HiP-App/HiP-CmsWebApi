using Api.Utility;
using BLL.Managers;
using BOL.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Data
{
    public static class DbSeed
    {
        // TODO: Move this code when seed data is implemented in EF 7

        public static void SeedDbWithAdministrator(this IApplicationBuilder app)
        {
            var dbContext = app.ApplicationServices.GetRequiredService<ApplicationDbContext>();
            var appConfig = app.ApplicationServices.GetRequiredService<AppConfig>();
            var userManager = new UserManager(dbContext);

            // Checking if the user exixts.
            var admin = userManager.GetUserByEmailAsync(appConfig.AdminEmail).Result;

            if (admin == null)
            {
                admin = new Administrator() { Email = appConfig.AdminEmail };
                var result = userManager.AddUserAsync(admin).Result;
            }
        }
    }
}
