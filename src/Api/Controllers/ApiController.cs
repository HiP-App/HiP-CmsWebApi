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

        protected static StatusCodeResult Conflict() => new StatusCodeResult(409);
    }
}
