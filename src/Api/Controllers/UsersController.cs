using Api.Utility;
using Microsoft.AspNetCore.Mvc;
using Api.Managers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.IO;
using Api.Data;
using Api.Models;
using Api.Models.User;
using Api.Permission;

namespace Api.Controllers
{
    public class UsersController : ApiController
    {
        private UserManager userManager;
        private EmailSender emailSender;
        private UserPermissions userPermissions;

        public UsersController(CmsDbContext dbContext, EmailSender emailSender, ILoggerFactory _logger) : base(dbContext, _logger)
        {
            userManager = new UserManager(dbContext);
            userPermissions = new UserPermissions(dbContext);
            this.emailSender = emailSender;
        }

        #region invite

        // POST api/users/invite
        [HttpPost("Invite")]
        [ProducesResponseType(typeof(void), 202)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 409)]
        [ProducesResponseType(typeof(void), 503)]
        public IActionResult Post(InviteFormModel model)
        {
            if (!userPermissions.IsAllowedToInvite(User.Identity.GetUserId()))
                return Forbidden();

            if (ModelState.IsValid)
            {
                int failCount = 0;
                foreach (string email in model.emails)
                {
                    try
                    {
                        userManager.AddUserbyEmail(email);
                        emailSender.InviteAsync(email);
                    }
                    //user already exists in Database
                    catch (Microsoft.EntityFrameworkCore.DbUpdateException)
                    {
                        failCount++;
                    }
                    //something went wrong when sending email
                    catch (MailKit.Net.Smtp.SmtpCommandException SmtpError)
                    {
                        _logger.LogDebug(SmtpError.ToString());
                        return ServiceUnavailable();
                    }
                }
                if (failCount == model.emails.Length)
                    return Conflict();
                
                return Accepted();
            }
            return BadRequest(ModelState);
        }

        #endregion

        #region GET user
        // GET api/users
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<UserResult>), 200)]
        public IActionResult Get(string query, string role, int page = 1)
        {
            var users = userManager.GetAllUsers(query, role, page, Constants.PageSize);
            int count = userManager.GetUsersCount();

            return Ok(new PagedResult<UserResult>(users, page, count));
        }


        // GET api/users/:id
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserResult), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult Get(int id)
        {
            var user = userManager.GetUserById(id);

            if (user != null)
                return Ok(new UserResult(user));
            else
                return NotFound();
        }


        // GET api/users/current
        [HttpGet("Current")]
        [ProducesResponseType(typeof(UserResult), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult CurrentUser()
        {
            var user = userManager.GetUserById(User.Identity.GetUserId());

            if (user != null)
                return Ok(new UserResult(user));
            else
                return NotFound();
        }
        #endregion

        #region PUT user
        // PUT api/users/current

        [HttpPut("Current")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult Put(UserFormModel model)
        {
            return PutUser(User.Identity.GetUserId(), model);
        }

        // PUT api/users/5
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult Put(int id, AdminUserFormModel model)
        {
            if (!userPermissions.IsAllowedToAdminister(User.Identity.GetUserId()))
                return Forbidden();

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
                    return NotFound();
                }
            }

            return BadRequest(ModelState);
        }

        #endregion

        #region GET picture

        // GET api/users/{userId}/picture/
        [HttpGet("{userId}/picture/")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult GetPictureById(int userId)
        {
            return GetPicture(userId);
        }

        // GET api/users/current/picture/
        [HttpGet("Current/picture/")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(void), 404)]
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
                    path = Path.Combine(Constants.ProfilePicturePath, Constants.DefaultPircture);

                return Ok(ToBase64String(path));
            }
            return NotFound();
        }

        #endregion

        #region POST picture


        // Post api/users/{id}/picture/
        [HttpPost("{id}/picture/")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 403)]
        public IActionResult PutPicture(int id, IFormFile file)
        {
            if (!userPermissions.IsAllowedToAdminister(User.Identity.GetUserId()))
                return Forbidden();
            return PutUserPicture(id, file);
        }

        // Post api/users/current/picture/
        [HttpPost("Current/picture/")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
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
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 403)]
        public IActionResult Delete(int id)
        {
            if (!userPermissions.IsAllowedToAdminister(User.Identity.GetUserId()))
                return Forbidden();
            return DeletePicture(id);
        }

        [HttpDelete("Current/picture/")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
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