namespace PaderbornUniversity.SILab.Hip.CmsApi.Utility
{
    public class AppConfig
    {
        public string EmailService { get; set; }

        /// <summary>
        /// Base URL of a running HiP-UserStore instance.
        /// Examples: "http://localhost:5000", "https://docker-hip.cs.upb.de/develop/userstore"
        /// </summary>
        public string UserStoreUrl { get; set; }
    }
}
