using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Produces("application/json")]
    [Route("Download")]
    [Authorize]
    [ProducesResponseType(typeof(void), 401)]
    public class DownloadController : Controller
    {
        public DownloadController()
        {

        }

        [HttpGet("{downloadHash}")]
        [ProducesResponseType(typeof(VirtualFileResult), 200)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult Get(string downloadHash)
        {
            bool isAllowed = true;
            if (!isAllowed)
                return ApiController.Forbidden();

            var resource = "...";
            if (resource != null)
            {
                var ip = HttpContext.Connection.RemoteIpAddress;
                string contentType = MimeKit.MimeTypes.GetMimeType(resource);
                return base.File(resource, contentType);
            }
            return NotFound();
        }
    }
}
