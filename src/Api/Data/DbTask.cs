﻿using Api.Utility;
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
