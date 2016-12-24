using Api.Utility;
using Api.Managers;
using Api.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Api.Models.Entity;
using System;
using Microsoft.AspNetCore.Hosting;

namespace Api.Data
{
    public static class DbTask
    {
        public static void RunMigrationsAndSeedDb(this IApplicationBuilder app, IHostingEnvironment env)
        {
            var dbContext = app.ApplicationServices.GetRequiredService<CmsDbContext>();
            var appConfig = app.ApplicationServices.GetRequiredService<AppConfig>();
            var userManager = new UserManager(dbContext);
            var setEnvironmentAsTest = "Test";

            //Basically we need the below check because the framework is using an In-memory database and hence it does not need the below call. 
            //Therefore we need a check checking if the environment is Test or not.
            //The default environment on which the tests are executed is 'Test'.
            //So only for this Test environment, the migrations won't apply and it will be applied for other environments. Eg: Production or Development or Staging
            if (!(env.EnvironmentName == setEnvironmentAsTest))
            {
                // Run Migrations to apply any pending migrations. (if any)
                dbContext.Database.Migrate();
            }

            // Seed Db with Admin Account if its not already created.
            try
            {
                userManager.GetUserByEmail(appConfig.AdminEmail);
            }
            catch (InvalidOperationException)
            {
                var admin = new User() { Email = appConfig.AdminEmail, Role = Role.Administrator };
                userManager.AddUser(admin);
            }
        }
    }
}
