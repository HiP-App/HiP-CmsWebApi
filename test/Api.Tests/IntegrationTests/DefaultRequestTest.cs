using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Api.Tests.IntegrationTests
{
    public class DefaultRequestTest
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public DefaultRequestTest()
        {
            // Arrange
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task TestSwagger()
        {
            // Act
            var response = await _client.GetAsync("/swagger/ui/index.html");
            response.EnsureSuccessStatusCode();

            // Assert
            Assert.True((System.Net.HttpStatusCode.OK == response.StatusCode));
        }
    }
}
