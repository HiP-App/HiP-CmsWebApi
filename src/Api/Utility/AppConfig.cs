using System.Text;
using Microsoft.Extensions.Configuration;

namespace Api.Utility
{
    public class AppConfig
    {
        public DatabaseConfig DatabaseConfig { get; set; }

        public AuthConfig AuthConfig { get; set; }

        public SMTPConfig SMTPConfig { get; set; }

        public bool RequireHttpsMetadata { get; set; }

        public string AdminEmail { get; set; }

        public AppConfig(IConfigurationRoot configuration)
        {
            DatabaseConfig = new DatabaseConfig
            {
                Host = configuration.GetValue<string>("DB_HOST"),
                Username = configuration.GetValue<string>("DB_USERNAME"),
                Password = configuration.GetValue<string>("DB_PASSWORD"),
                Name = configuration.GetValue<string>("DB_NAME")
            };

            AuthConfig = new AuthConfig 
            {
                ClientId = configuration.GetValue<string>("CLIENT_ID"),
                Domain = configuration.GetValue<string>("DOMAIN")
            };

            SMTPConfig = new SMTPConfig
            {
                From = configuration.GetValue<string>("SMTP_FROM"),
                Server = configuration.GetValue<string>("SMTP_SERVER"),
                Port = configuration.GetValue<int>("SMTP_PORT"),
                WithSSL = configuration.GetValue<bool>("SMTP_WITH_SSL"),
                User = configuration.GetValue<string>("SMTP_USER"),
                Password = configuration.GetValue<string>("SMTP_PASSWORD")
            };

            RequireHttpsMetadata = !configuration.GetValue<bool>("ALLOW_HTTP");
            AdminEmail = configuration.GetValue<string>("ADMIN_EMAIL");
        }
    }


    public class DatabaseConfig
    {
        public string Name { get; set; }

        public string Host { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string ConnectionString { 
            get {
                StringBuilder connectionString = new StringBuilder();
                
                connectionString.Append($"Host={Host};");
                connectionString.Append($"Username={Username};");
                connectionString.Append($"Password={Password};");
                connectionString.Append($"Database={Name};");
                connectionString.Append($"Pooling=true;");

                return connectionString.ToString();
            }
        }
    }

    public class AuthConfig
    {
        public string ClientId { get; set; }

        public string Domain { get; set; }
    }

    public class SMTPConfig
    {
        public string From { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
        public bool WithSSL { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }
}
