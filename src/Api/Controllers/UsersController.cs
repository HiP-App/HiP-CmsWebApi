using Api.Utility;
using BOL.Models;
using Microsoft.AspNetCore.Mvc;
using BLL.Managers;
using Api.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{
    public class UsersController : ApiController
    {
        private UserManager userManager;

        public UsersController(ApplicationDbContext dbContext, ILoggerFactory _logger) : base(dbContext, _logger)
        {
            userManager = new UserManager(dbContext);
        }


        // GET api/users
        [HttpGet]
        public async Task<IActionResult> Get(string query, string role, int page = 1)
        {
            var users = await userManager.GetAllUsersAsync(query, role, page, Constants.PageSize);
            int count = await userManager.GetUsersCountAsync();

            return Ok(new PagedResult<User>(users, page, count));
        }


        // GET api/users/:id
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await userManager.GetUserByIdAsync(id);

            if (user != null)
                return Ok(user);
            else
                return NotFound();             
        }


        // GET api/users/current
        [HttpGet]
        [Route("Current")]
        public async Task<IActionResult> CurrentUser()
        {
            var user = await userManager.GetUserByIdAsync(User.Identity.GetUserId());

            if (user != null)
                return Ok(user);
            else
                return NotFound();             
        }


        // PUT api/values/5
        [HttpPut("{id}")]
        [Authorize(Roles = Role.Administrator)]
        public async Task<IActionResult> Put(int id, UserFormModel model)
        {
            if(ModelState.IsValid)
            {
                if (!Role.IsRoleValid(model.Role))
                {
                    ModelState.AddModelError("Role", "Invalid Role");
                }                   
                else
                {
                    if(await userManager.UpdateUserAsync(id, model))
                    {
                        _logger.LogInformation(5, "User with ID: " + id + " updated.");

                        return Ok();
                    }
                }
            }

            return BadRequest(ModelState);
        }
    }
}