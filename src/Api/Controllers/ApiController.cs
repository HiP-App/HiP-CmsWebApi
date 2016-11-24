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
            return Convert.ToBase64String(System.IO.File.ReadAllBytes(source));
        }

        public static StatusCodeResult Forbidden() { return new StatusCodeResult(403); }
        public static StatusCodeResult Conflict() { return new StatusCodeResult(409); }
        public static StatusCodeResult Gone() { return new StatusCodeResult(410); }
        public static StatusCodeResult ServiceUnavailable() { return new StatusCodeResult(503); }
        public static ObjectResult InternalServerError(Object error) { return new ObjectResult(error) { StatusCode = 500 }; }


    }

}
