using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.CmsApi
{
    public abstract class NSwagClientBase
    {
        /// <summary>
        /// The value for the HTTP Authorization header,
        /// e.g. "Bearer [Your JWT token here]".
        /// </summary>
        public string Authorization { get; set; }

        protected Task<HttpClient> CreateHttpClientAsync(CancellationToken cancellationToken)
        {
            var http = new HttpClient();
            if (!string.IsNullOrEmpty(Authorization))
                http.DefaultRequestHeaders.Add("Authorization", Authorization);
            return Task.FromResult(http);
        }
    }
}