// ReSharper disable RedundantUsingDirective
using PaderbornUniversity.SILab.Hip.CmsApi.Utility;
using PaderbornUniversity.SILab.Hip.CmsApi.Managers;
using PaderbornUniversity.SILab.Hip.CmsApi.Models;
using Microsoft.AspNetCore.Builder;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Data
{
    public static class DbTask
    {
        public static void RunMigrationsAndSeedDb(this IApplicationBuilder app)
        {
            var dbContext = app.ApplicationServices.GetRequiredService<CmsDbContext>();
            var appConfig = app.ApplicationServices.GetRequiredService<AppConfig>();
            var userManager = new UserManager(dbContext);

            // Run Migrations to apply any pending migrations. (if any)
            dbContext.Database.Migrate();

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
