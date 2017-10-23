namespace PaderbornUniversity.SILab.Hip.CmsApi.Utility
{
    public class DatabaseConfig
    {
        public string Name { get; set; }

        public string Host { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Port { get; set; }

        public string ConnectionString =>
            $"Host={Host};" +
            $"Username={Username};" +
            $"Password={Password};" +
            $"Database={Name};" +
            $"Port=5632;" +
            $"Pooling=true;" +
            $"Port={Port};";
    }
}
