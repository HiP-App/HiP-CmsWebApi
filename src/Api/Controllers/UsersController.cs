using Api.Utility;
using Api.ViewModels.Users;
using BOL.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BLL.Managers;
using Api.Data;
using System.Threading.Tasks;

namespace Api.Controllers
{
    public class UsersController : ApiController
    {
        private UserManager userManager;

        public UsersController(ApplicationDbContext dbContext) : base(dbContext)
        {
            userManager = new UserManager(dbContext);
        }

        // GET: api/users
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
                    ModelState.AddModelError("Role", "Invalid Role");
                else
                {
                    bool success = await userManager.UpdateUserRoleAsync(id, model.Role);

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
