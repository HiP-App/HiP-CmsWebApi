using Api.Data;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ApiController : Controller
    {
        protected readonly ApplicationDbContext dbContext;

        public ApiController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}
