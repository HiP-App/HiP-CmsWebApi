using Api.Data;
using Api.Utility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.Swagger.Model;
using System;
using System.IO;
using Api.Services;

namespace Api
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
            // Read configurations from json
            var appConfig = new AppConfig(Configuration);

            // Register AppConfig in Services 
            services.AddSingleton(appConfig);
            services.AddTransient<IEmailSender, EmailSender>();

            // Adding Cross Orign Requests 
            services.AddCors();

            // Add database service for Postgres
            services.AddDbContext<CmsDbContext>(options => options.UseNpgsql(appConfig.DatabaseConfig.ConnectionString));

            // Add framework services.
            services.AddMvc();

            // Add Swagger service
            services.AddSwaggerGen();

            // Configurig Metadata in Swagger
            services.ConfigureSwaggerGen(options =>
            {
                options.SingleApiVersion(new Info
                {
                    Version = "v1",
                    Title = "HiPCMS API",
                    Description = "A REST api to serve History in Paderborn CMS System"
                });
                //Set the comments path for the swagger json and ui.
                options.IncludeXmlComments(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "Api.xml"));
                options.OperationFilter<SwaggerOperationFilter>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, AppConfig appConfig)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            if (env.IsDevelopment())
                loggerFactory.AddDebug();

            app.UseCors(builder =>
                // This will allow any request from any server. Tweak to fit your needs!
                builder.AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowAnyOrigin()
            );

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                Audience = appConfig.AuthConfig.ClientId,
                Authority = appConfig.AuthConfig.Domain,
                AutomaticChallenge = true,
                AutomaticAuthenticate = true,
                RequireHttpsMetadata = appConfig.RequireHttpsMetadata,
                Events = new CmsApuJwtBearerEvents(app.ApplicationServices.GetRequiredService<CmsDbContext>())
            });

            app.UseMvc();

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger();
            app.UseSwaggerUi();

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
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
