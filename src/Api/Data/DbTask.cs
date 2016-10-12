using Api.Utility;
using BLL.Managers;
using BOL.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Data
{
    public static class DbTask
    {
        public static void RunMigrationsAndSeedDb(this IApplicationBuilder app)
        {
            var dbContext = app.ApplicationServices.GetRequiredService<ApplicationDbContext>();
            var appConfig = app.ApplicationServices.GetRequiredService<AppConfig>();
            var userManager = new UserManager(dbContext);

            // Run Migrations to apply any pending migrations. (if any)
            dbContext.Database.Migrate();

            // Seed Db with Admin Account if its not already created.
            var admin = userManager.GetUserByEmail(appConfig.AdminEmail);

            if (admin == null)
            {
                admin = new Administrator() { Email = appConfig.AdminEmail };
                userManager.AddUser(admin);
            }
        }
    }
}
