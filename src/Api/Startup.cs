using Api.Data;
using Api.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using MySQL.Data.EntityFrameworkCore.Extensions;
using Swashbuckle.Swagger.Model;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Api
{
    public class Startup
    {
        public static readonly string ProfilePictureFolder = @"wwwroot/profilepictures";

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Read configurations from json
            var appConfig = new AppConfig(Configuration);

            // Register AppConfig in Services 
            services.AddSingleton(appConfig);

            // Adding Cross Orign Requests 
            services.AddCors();

            // Add database service for Postgres
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySQL(appConfig.DatabaseConfig.ConnectionString));

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
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, AppConfig appConfig)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
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
                
                Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        return Task.FromResult(0);
                    },

                    OnTokenValidated = context =>
                    {
                        Auth.OnTokenValidationSuccess(
                            context.Ticket.Principal.Identity as ClaimsIdentity, 
                            app.ApplicationServices.GetRequiredService<ApplicationDbContext>());

                        return Task.FromResult(0);
                    }
                }
            });

            app.UseMvc();

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger();

            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUi();

            // Run all pending Migrations and Seed DB with initial data
            app.RunMigrationsAndSeedDb();

            // Use Static files eg for profile pictures
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), ProfilePictureFolder)),
                RequestPath = new PathString("/ProfilePictures")
            });
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
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
