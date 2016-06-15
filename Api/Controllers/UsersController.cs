using Api.Data;
using Api.Utility;
using Api.ViewModels.Users;
using BOL.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    public class UsersController : ApiController
    {
        public UsersController(CmsDbContext db) : base(db) {}

        // GET: api/users
        [HttpGet]
        public IActionResult Get(string query, string role, int page = 1)
        {
            var users = from u in db.Users
                        select u;
            
            if(!string.IsNullOrEmpty(query))
                users = users.Where(u =>
                    u.Email.Contains(query) ||
                    u.FirstName.Contains(query) ||
                    u.LastName.Contains(query));

            if (!string.IsNullOrEmpty(role))
                users = users.Where(u => u.Role == role);

            return Ok(new PagedResult<User>(users, page, db.Users.Count()));
        }

        // GET api/users/:id
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var user = db.Users.FirstOrDefault(u => u.Id == id);

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
                    var user = db.Users.FirstOrDefault(u => u.Id == id);
                    db.Database.ExecuteSqlCommand($"UPDATE \"User\" SET \"Role\" = '{model.Role}' where \"Id\" = {id}");

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
