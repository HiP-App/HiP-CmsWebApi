using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Tests
{
    public class TestStartup : Startup
    {
        public TestStartup(IHostingEnvironment env) : base(env) { }

        public void ConfigureTestServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            // Register here the dependency injection services
            // services.AddDbContext<ApplicationDbContext>(opts => opts.UseInMemoryDatabase());
            // Not required because the framework internally uses InMemoryDatabase
            //https://github.com/ivaylokenov/MyTested.AspNetCore.Mvc/blob/development/samples/MusicStore/MusicStore.Test/TestStartup.cs
        }
    }
}
