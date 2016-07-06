using Api.Utility;
using Api.ViewModels.Users;
using BOL.Models;
using Microsoft.AspNetCore.Mvc;
using BLL.Managers;
using Api.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    public class UsersController : ApiController
    {
        private UserManager userManager;

        public UsersController(ApplicationDbContext dbContext, ILoggerFactory _logger) : base(dbContext, _logger)
        {
            userManager = new UserManager(dbContext);
        }

        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> Get(string query, string role, int page = 1)
        {
            var users = await userManager.GetAllUsersAsync(query, role, page, Constants.PageSize);
            int count = await userManager.GetUsersCountAsync();

            _logger.LogInformation(1, "Number of Users successfully retrieved: " + count);
            return Ok(new PagedResult<User>(users, page, count));
        }

        // GET api/users/:id
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await userManager.GetUserByIdAsync(id);

            if (user != null)
            {
                _logger.LogInformation(2, "The User Id: " + id + " exists");
                return Ok(user);
            }               
            else
            {
                _logger.LogInformation(3, "The User Id: " + id + " does not exists");
                return NotFound();
            }                
        }

        // POST api/users
        [HttpPost]
        public IActionResult Post()
        {
            return NotFound(); 
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, ChangeRoleModel model)
        {
            if(ModelState.IsValid)
            {
                if (!Role.IsRoleValid(model.Role))
                {
                    _logger.LogInformation(4, "The Role: " + model.Role + " is not valid");
                    ModelState.AddModelError("Role", "Invalid Role");
                }                   
                else
                {
                    bool success = await userManager.UpdateUserRoleAsync(id, model.Role);
                    _logger.LogInformation(5, "The information for : " + id + " successfully updated");

                    if(success)
                        return Ok();
                }
            }

            return BadRequest(ModelState);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return NotFound();
        }
    }
}
