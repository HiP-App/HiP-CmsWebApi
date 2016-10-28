using Api.Utility;
using Api.Models.Entity;
using Microsoft.AspNetCore.Mvc;
using Api.Managers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;
using Api.Data;
using Api.Models;

namespace Api.Controllers
{
    public class UsersController : ApiController
    {
        private UserManager userManager;
        private EmailSender emailSender;

        public UsersController(CmsDbContext dbContext, EmailSender emailSender, ILoggerFactory _logger) : base(dbContext, _logger)
        {
            userManager = new UserManager(dbContext);
            this.emailSender = emailSender;
        }

        // POST api/users/invite
        [HttpPost("Invite")]
        [Authorize(Roles = Role.Supervisor + "," + Role.Administrator)]
        public IActionResult Post(InviteFormModel model)
        {
            if (ModelState.IsValid)
            {
                foreach (string email in model.emails)
                {
                    emailSender.InviteAsync(email);
                }
                return Ok();
            }
            return BadRequest(ModelState);
        }

        #region GET user
        // GET api/users
        [HttpGet]
        public IActionResult Get(string query, string role, int page = 1)
        {
            var users = userManager.GetAllUsers(query, role, page, Constants.PageSize);
            int count = userManager.GetUsersCount();

            return Ok(new PagedResult<User>(users, page, count));
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


        // GET api/users/current
        [HttpGet]
        [Route("Current")]
        public IActionResult CurrentUser()
        {
            var user = userManager.GetUserById(User.Identity.GetUserId());

            if (user != null)
                return Ok(user);
            else
                return NotFound();
        }
        #endregion

        #region PUT user
        // PUT api/users/current

        [HttpPut("Current")]
        public IActionResult Put(UserFormModel model)
        {
            return PutUser(User.Identity.GetUserId(), model);
        }

        // PUT api/users/5
        [HttpPut("{id}")]
        [Authorize(Roles = Role.Administrator)]
        public IActionResult Put(int id, AdminUserFormModel model)
        {
            return PutUser(id, model);
        }

        private IActionResult PutUser(int id, UserFormModel model)
        {
            if (ModelState.IsValid)
            {
                if (model is AdminUserFormModel && !Role.IsRoleValid(((AdminUserFormModel)model).Role))
                {
                    ModelState.AddModelError("Role", "Invalid Role");
                }
                else
                {
                    if (userManager.UpdateUser(id, model))
                    {
                        _logger.LogInformation(5, "User with ID: " + id + " updated.");
                        return Ok();
                    }
                }
            }

            return BadRequest(ModelState);
        }

        #endregion

        #region GET picture

        // GET api/users/{userId}/picture/
        [HttpGet("{userId}/picture/")]
        public IActionResult GetPictureById(int userId)
        {
            return GetPicture(userId);
        }

        // GET api/users/current/picture/
        [HttpGet]
        [Route("Current/picture/")]
        public IActionResult GetPictureForCurrentUser()
        {
            return GetPicture(User.Identity.GetUserId());
        }

        private IActionResult GetPicture(int userId)
        {
            var user = userManager.GetUserById(userId);
            if (user != null)
            {
                string path = Path.Combine(Constants.ProfilePicturePath, user.Picture);
                if (!System.IO.File.Exists(path))
                    return NotFound();

                string contentType = MimeKit.MimeTypes.GetMimeType(path);
                return base.File(Path.Combine(Constants.ProfilePictureFolder, user.Picture), contentType);
            }
            return BadRequest();
        }

        #endregion

        #region POST picture


        // Post api/users/{id}/picture/
        [HttpPost("{id}/picture/")]
        [Authorize(Roles = Role.Administrator)]
        public IActionResult PutPicture(int id, IFormFile file)
        {
            return PutUserPicture(id, file);
        }

        // Post api/users/current/picture/
        [HttpPost("Current/picture/")]
        public IActionResult PutPicture(IFormFile file)
        {
            return PutUserPicture(User.Identity.GetUserId(), file);
        }

        private IActionResult PutUserPicture(int userId, IFormFile file)
        {
            var uploads = Path.Combine(Constants.ProfilePicturePath);
            var user = userManager.GetUserById(userId);

            if (file == null)
                ModelState.AddModelError("file", "File is null");
            else if (user == null)
                ModelState.AddModelError("userId", "Unkonown User");
            else if (file.Length > 1024 * 1024 * 5) // Limit to 5 MB
                ModelState.AddModelError("file", "Picture is to large");
            else if (IsImage(file))
            {
                string fileName = user.Id + Path.GetExtension(file.FileName);
                DeleteFile(Path.Combine(uploads, fileName));

                using (FileStream outputStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                {
                    file.CopyTo(outputStream);
                }

                userManager.UpdateProfilePicture(user, fileName);
                return Ok();
            }
            else
                ModelState.AddModelError("file", "Invalid Image");

            return BadRequest(ModelState);
        }

        private bool IsImage(IFormFile file)
        {
            return ((file != null) && System.Text.RegularExpressions.Regex.IsMatch(file.ContentType, "image/\\S+") && (file.Length > 0));
        }

        #endregion

        #region DELETE picture

        [HttpDelete("{id}/picture/")]
        [Authorize(Roles = Role.Administrator)]
        public IActionResult Delete(int id)
        {
            return DeletePicture(id);
        }

        [HttpDelete("Current/picture/")]
        public IActionResult Delete()
        {
            return DeletePicture(User.Identity.GetUserId());
        }

        private IActionResult DeletePicture(int userId)
        {
            // Fetch user
            var user = userManager.GetUserById(userId);
            if (user == null)
                return BadRequest("Could not find User");
            // Has A Picture?
            if (!user.HasProfilePicture())
                return BadRequest("No picture set");

            bool success = userManager.UpdateProfilePicture(user, "");
            // Delete Picture If Exists
            string fileName = Path.Combine(Constants.ProfilePicturePath, user.Picture);

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