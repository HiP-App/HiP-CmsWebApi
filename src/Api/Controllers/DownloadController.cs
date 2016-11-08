using Api.Managers;
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
    public class DownloadController : Controller
    {
        public DownloadController()
        {

        }

        [HttpGet("{downloadHash}")]
        [ProducesResponseType(typeof(VirtualFileResult), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult Get(string downloadHash)
        {
            var userIp = HttpContext.Connection.RemoteIpAddress;
            var resource = DownloadManager.GetResource(downloadHash, userIp);
            if (resource != null)
            {
                string contentType = MimeKit.MimeTypes.GetMimeType(resource.FileName);
                return base.File(resource.FileName, contentType);
            }
            return NotFound();
        }
    }
}
