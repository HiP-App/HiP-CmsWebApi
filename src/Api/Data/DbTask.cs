// ReSharper disable RedundantUsingDirective
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Data
{
    public static class DbTask
    {
        public static void RunMigrationsAndSeedDb(this IApplicationBuilder app)
        {
            var dbContext = app.ApplicationServices.GetRequiredService<CmsDbContext>();

            // Run Migrations to apply any pending migrations. (if any)
            dbContext.Database.Migrate();
        }
    }
}
