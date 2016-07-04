using Microsoft.Extensions.Configuration;

namespace Api.Utility
{
    public class AppConfig
    {
        public string ClientId { get; set; }

        public string Domain { get; set; }

        public bool RequireHttpsMetadata { get; set; }

        public string AdminEmail { get; set; }

        public AppConfig(IConfigurationRoot configuration)
        {
            ClientId = configuration.GetValue<string>("Configurations:Auth:ClientId");
            Domain = configuration.GetValue<string>("Configurations:Auth:Domain");
            RequireHttpsMetadata = configuration.GetValue<bool>("Configurations:RequireHttpsMetadata");
            AdminEmail = configuration.GetValue<string>("Configurations:AdminEmail");
        }
    }
}
