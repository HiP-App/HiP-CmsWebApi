using Api.Managers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MyTested.AspNetCore.Mvc;
using Api.Tests.Nunit.Mocks;
using Api.Permission;

namespace Api.Tests.Nunit
{
    public class TestStartup : Startup
    {
        public TestStartup(IHostingEnvironment env) : base(env) { }

        public void ConfigureTestServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
        }
    }
}
