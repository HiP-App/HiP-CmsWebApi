using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using PaderbornUniversity.SILab.Hip.CmsApi.Data;
using PaderbornUniversity.SILab.Hip.CmsApi.Managers;
using PaderbornUniversity.SILab.Hip.CmsApi.Permission;
using PaderbornUniversity.SILab.Hip.CmsApi.Services;
using PaderbornUniversity.SILab.Hip.CmsApi.Utility;
using PaderbornUniversity.SILab.Hip.Webservice;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using AppConfig = PaderbornUniversity.SILab.Hip.CmsApi.Utility.AppConfig;

namespace PaderbornUniversity.SILab.Hip.CmsApi
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Startup
    {
        internal static IServiceProvider ServiceProvider { get; private set; }

        private IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        /// <summary>
        /// Configures the built-in container's services, i.e. the services added to the IServiceCollection
        /// parameter are available via dependency injection afterwards.
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Read configurations from JSON or environment variables
            services
                .Configure<AppConfig>(Configuration.GetSection("App"))
                .Configure<DatabaseConfig>(Configuration.GetSection("Database"))
                .Configure<AuthConfig>(Configuration.GetSection("Auth"));

            var serviceProvider = services.BuildServiceProvider();
            var appConfig = serviceProvider.GetService<IOptions<AppConfig>>().Value;
            var authConfig = serviceProvider.GetService< IOptions<AuthConfig>>().Value;
            var databaseConfig = serviceProvider.GetService< IOptions<DatabaseConfig>>().Value;

            // Register AppConfig in Services 
            services
                .AddTransient<IEmailSender, EmailSender>()
                .AddScoped<UserManager>()
                .AddScoped<NotificationManager>()
                .AddScoped<TopicManager>()
                .AddScoped<AttachmentsManager>()
                .AddScoped<DocumentManager>()
                .AddScoped<ContentAnalyticsManager>()
                .AddScoped<AnnotationPermissions>()
                .AddScoped<TopicPermissions>()
                .AddScoped<UserPermissions>();

            // Configure authentication
            services
                .AddAuthentication(options => options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Audience = authConfig.Audience;
                    options.Authority = authConfig.Authority;
                });

            // Configure authorization
            var domain = authConfig.Authority;

            services.AddAuthorization(options =>
            {
                options.AddPolicy("read:webapi",
                    policy => policy.Requirements.Add(new HasScopeRequirement("read:webapi", domain)));
                options.AddPolicy("write:webapi",
                    policy => policy.Requirements.Add(new HasScopeRequirement("write:webapi", domain)));
            });

            // Adding Cross Orign Requests 
            services.AddCors();

            // Add database service for Postgres
            services.AddDbContext<CmsDbContext>(options => options.UseNpgsql(databaseConfig.ConnectionString));

            // Add framework services.
            services.AddMvc();

            // Add Swagger service
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "HiPCMS API", Version = "v1", Description = "A REST api to serve History in Paderborn CMS System" });
                c.IncludeXmlComments(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "Api.xml"));
                c.OperationFilter<CustomSwaggerOperationFilter>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            if (env.IsDevelopment())
                loggerFactory.AddDebug();

            app.UseCors(builder => 
            {
                // This will allow any request from any server. Tweak to fit your needs!
                builder.AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowAnyOrigin();
            });

            app.UseAuthentication();
            app.UseMvc();

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swaggerDoc, httpReq) => swaggerDoc.Host = httpReq.Host.Value);
            });
            app.UseSwaggerUI(c =>
            {
                // Only a hack, if HiP-Swagger is running, SwaggerUI can be disabled for Production
                c.SwaggerEndpoint((env.IsDevelopment() ? "/swagger" : "..") + "/v1/swagger.json", "HiPCMS API V1");
            });

            // Run all pending Migrations and Seed DB with initial data
            app.RunMigrationsAndSeedDb();
            app.UseStaticFiles();

            ServiceProvider = app.ApplicationServices;
        }

        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseConfiguration(config)
                .UseUrls("http://*:5001")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
