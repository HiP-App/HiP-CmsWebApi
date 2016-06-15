using Api.Data;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class ApiController : Controller
    {
        protected readonly CmsDbContext db;

        public ApiController(CmsDbContext db)
        {
            this.db = db;
        }
    }
}
