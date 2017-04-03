using PaderbornUniversity.SILab.Hip.CmsApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Controllers
{
    /// <summary>
    /// A base controller for handling user requests
    /// </summary>    
    /// <response code="401">User is denied</response>
    [Produces("application/json")]
    [Route("Api/[controller]")]
    [Authorize]
    [ProducesResponseType(typeof(void), 401)]
    public class ApiController : Controller
    {
        protected readonly CmsDbContext DbContext;
        protected readonly ILogger Logger;

        public ApiController(CmsDbContext dbContext, ILoggerFactory loggerFactory)
        {
            DbContext = dbContext;
            Logger = loggerFactory.CreateLogger<ApiController>();
        }

        protected static string ToBase64String(string source)
        {
            return Convert.ToBase64String(System.IO.File.ReadAllBytes(source));
        }

        public static StatusCodeResult Forbidden() { return new StatusCodeResult(403); }
        protected static StatusCodeResult Conflict() { return new StatusCodeResult(409); }
        public static StatusCodeResult Gone() { return new StatusCodeResult(410); }
        protected static StatusCodeResult ServiceUnavailable() { return new StatusCodeResult(503); }
        protected static ObjectResult InternalServerError(object error) { return new ObjectResult(error) { StatusCode = 500 }; }


    }

}
