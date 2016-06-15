using Api.Utility;
using Api.ViewModels.Users;
using BOL.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BLL.Managers;
using Api.Data;

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
        public IActionResult Get(string query, string role, int page = 1)
        {
            return Ok(new PagedResult<User>(userManager.GetAllUsers(), page, userManager.GetUsersCount()));
        }

        // GET api/users/:id
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var user = userManager.GetUserById(id);

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
        public IActionResult Put(int id, ChangeRoleModel model)
        {
            if(ModelState.IsValid)
            {
                if (!Role.IsRoleValid(model.Role))
                    ModelState.AddModelError("Role", "Invalid Role");
                else
                {
                    userManager.UpdateUserRole(id, model.Role);
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
