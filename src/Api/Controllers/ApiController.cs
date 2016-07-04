using Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ApiController : Controller
    {
        protected readonly ApplicationDbContext dbContext;
        protected readonly ILogger _logger;

        public ApiController(ApplicationDbContext dbContext, ILoggerFactory loggerFactory)
        {
            this.dbContext = dbContext;
            _logger = loggerFactory.CreateLogger<ApiController>();
        }
    }
}
