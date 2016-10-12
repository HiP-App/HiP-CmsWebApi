using Api.Utility;
using BOL.Models;
using Microsoft.AspNetCore.Mvc;
using BLL.Managers;
using Api.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Razor.CodeGenerators;
using Microsoft.AspNetCore.StaticFiles;


namespace Api.Controllers
{
    public class UsersController : ApiController
    {
        private UserManager userManager;

        public UsersController(ApplicationDbContext dbContext, ILoggerFactory _logger) : base(dbContext, _logger)
        {
            userManager = new UserManager(dbContext);
        }

        #region GET user
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
        #endregion

        #region PUT user
        // PUT api/users/current

        [HttpPut("Current")]
        public async Task<IActionResult> Put(UserFormModel model)
        {
            return await PutUser(User.Identity.GetUserId(), model);
        }

        // PUT api/users/5
        [HttpPut("{id}")]
        [Authorize(Roles = Role.Administrator)]
        public async Task<IActionResult> Put(int id, AdminUserFormModel model)
        {
            return await PutUser(id, model);
        }

        private async Task<IActionResult> PutUser(int id, UserFormModel model)
        {
            if (ModelState.IsValid)
            {
                if (model is AdminUserFormModel && !Role.IsRoleValid(((AdminUserFormModel) model).Role))
                {
                    ModelState.AddModelError("Role", "Invalid Role");
                }
                else
                {
                    if (await userManager.UpdateUserAsync(id, model))
                    {
                        _logger.LogInformation(5, "User with ID: " + id + " updated.");
                        return Ok();
                    }
                }
            }

            return BadRequest(ModelState);
        }

        #endregion

        #region POST picture

        // Post api/users/{id}/picture/
        [HttpPost("{id}/picture/")]
        [Authorize(Roles = Role.Administrator)]
        public async Task<IActionResult> PutPicture(int id, IFormFile file)
        {
            return await PutUserPicture(id, file);
        }

        // Post api/users/current/picture/
        [HttpPost("Current/picture/")]
        public async Task<IActionResult> PutPicture(IFormFile file)
        {
            return await PutUserPicture(User.Identity.GetUserId(), file);
        }

        private async Task<IActionResult> PutUserPicture(int userId, IFormFile file)
        {
            if (file == null) return BadRequest("File is null");
            if (file.Length == 0) return BadRequest("File is empty");

            var uploads = Path.Combine(Directory.GetCurrentDirectory(), Startup.ProfilePictureFolder);
            var user = await userManager.GetUserByIdAsync(userId);

            if (user == null) return BadRequest("Unkonown User");

            //if (file.Length > 1024 * 1024) // Limit to 1 MB
            //    return BadRequest("Picture is to large");
            if (file.Length > 0)
            {
                string fileName = user.Id + Path.GetExtension(file.FileName);
                DeleteFile(Path.Combine(uploads, fileName));

                using (FileStream outputStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                {
                    file.CopyTo(outputStream);
                }

                await userManager.UpdateProfilePicture(user, fileName);

                return Ok();
            }
            return BadRequest(ModelState);
        }

        #endregion

        #region DELETE picture

        [HttpDelete("{id}/picture/")]
        [Authorize(Roles = Role.Administrator)]
        public async Task<IActionResult> Delete(int id)
        {
            return await DeletePicture(id);
        }

        [HttpDelete("Current/picture/")]
        public async Task<IActionResult> Delete()
        {
            return await DeletePicture(User.Identity.GetUserId());
        }

        private async Task<IActionResult> DeletePicture(int userId)
        {
            // Fetch user
            var user = await userManager.GetUserByIdAsync(userId);
            if (user == null)
                return BadRequest("Could not find User");
            // Has A Picture?
            if (!user.HasProfilePicture())
                return BadRequest("No picture set");

            bool success = await userManager.UpdateProfilePicture(user, "");
            // Delete Picture If Exists
            string fileName = Path.Combine(Directory.GetCurrentDirectory(), Startup.ProfilePictureFolder, user.Picture);

            DeleteFile(fileName);

            if (success)
                return Ok();
            else
                return BadRequest();
        }

        private void DeleteFile(string path)
        {
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
        }

        #endregion
    }

}