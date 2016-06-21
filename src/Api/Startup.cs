using Api.Data;
using BOL.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Adding Cross Orign Requests 
            services.AddCors();

            // Add database service for Postgres
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            // Add framework services.
            services.AddMvc();

            // Add Swagger service
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, ApplicationDbContext db)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseCors(builder =>
                // This will allow any request from any server. Tweak to fit your needs!
                // The fluent API is pretty pleasant to work with.
                builder.AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin()
            );
            
            var a = Configuration.GetSection("Auth:ClientId").Value;
            var b = Configuration.GetSection("Auth:Domain").Value;

            //For Seeding the User
            StartupTasks user = new StartupTasks(db);

            //Seed the Database with the Administrator
            user.CreateUser(Configuration.GetSection("AppCredentials:Admin:Username").Value, Role.Administrator);

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                Audience = a,
                Authority = b,
                AutomaticChallenge = true,
                AutomaticAuthenticate = true,
                RequireHttpsMetadata = false,
                Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        return Task.FromResult(0);
                    },

                    OnTokenValidated = context =>
                    {
                        var claimsIdentity = context.Ticket.Principal.Identity as ClaimsIdentity;
                        claimsIdentity.AddClaim(new Claim("id_token",
                            context.Request.Headers["Authorization"][0].Substring(context.Ticket.AuthenticationScheme.Length + 1)));
                        
                        //Adding User to the database if not eists
                        user.CheckandCreateUser(claimsIdentity);

                        return Task.FromResult(0);
                    }
                }
            });

            app.UseMvc();

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwaggerGen();
            
            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUi();
            
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
