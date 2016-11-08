using Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Text;

namespace Api.Controllers
{
    [Produces("application/json")]
    [Route("Api/[controller]")]
    [Authorize]
    [ProducesResponseType(typeof(void), 401)]
    public class ApiController : Controller
    {
        protected readonly CmsDbContext dbContext;
        protected readonly ILogger _logger;

        public ApiController(CmsDbContext dbContext, ILoggerFactory loggerFactory)
        {
            this.dbContext = dbContext;
            _logger = loggerFactory.CreateLogger<ApiController>();
        }

        protected static String ToBase64String(string source)
        {
            return Convert.ToBase64String(Encoding.Unicode.GetBytes(source));
        }

        protected StatusCodeResult Accepted() { return new StatusCodeResult(202); }
        protected StatusCodeResult Forbidden() { return new StatusCodeResult(403); }
        protected StatusCodeResult Conflict() { return new StatusCodeResult(409); }
        protected StatusCodeResult ServiceUnavailable() { return new StatusCodeResult(503); }

        protected ObjectResult InternalServerError(Object error) { return new ObjectResult(error) { StatusCode = 500 }; }
         
        
    }

}
