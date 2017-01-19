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
using System;
using System.Collections.Generic;

namespace Api.Controllers
{
    public class UsersController : ApiController
    {
        private UserManager userManager;
        private UserPermissions userPermissions;

        public UsersController(CmsDbContext dbContext, ILoggerFactory _logger) : base(dbContext, _logger)
        {
            userManager = new UserManager(dbContext);
            userPermissions = new UserPermissions(dbContext);
        }

        #region invite

        // POST api/users/invite

        /// <summary>
        /// Add new users and send invitation to the added users for registration
        /// </summary>        
        /// <param name="model">Contains a list of emails</param>                         
        /// <response code="202">Request is accepted</response>        
        /// <response code="400">Request incorrect</response>        
        /// <response code="403">User not allowed to invite new users</response>        
        /// <response code="409">Resource already exists</response>        
        /// <response code="503">Service unavailable</response>        
        /// <response code="401">User is denied</response>
        [HttpPost("Invite")]
        [ProducesResponseType(typeof(UserManager.InvitationResult), 202)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(UserManager.InvitationResult), 409)]
        [ProducesResponseType(typeof(void), 503)]
        public IActionResult InviteUsers([FromBody]InviteFormModel model, [FromServices]IEmailSender emailSender)
        {
            if (!userPermissions.IsAllowedToInvite(User.Identity.GetUserId()))
                return Forbidden();

            if (ModelState.IsValid)
            {
                UserManager.InvitationResult result = userManager.InviteUsers(model.emails, emailSender);
                if (result.failedInvitations.Count == model.emails.Length)
                    return BadRequest(result);
                if (result.existingUsers.Count == model.emails.Length)
                    return StatusCode(409, result);

                return Accepted(result);
            } else
            {
                return BadRequest(ModelState);
            }
        }

        #endregion

        #region GET user

        // GET api/users

        /// <summary>
        /// All users matching query and role
        /// </summary>   
        /// <param name="query">Users containing query in email, first and last name</param>
        /// <param name="role">Represents role of the user</param>        
        /// <param name="page">Represents the page</param>
        /// <response code="200">Returns PagedResults of UserResults</response>        
        /// <response code="401">User is denied</response>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<UserResult>), 200)]
        public IActionResult Get(string query, string role, int page = 1)
        {
            var users = userManager.GetAllUsers(query, role, page, Constants.PageSize);
            int count = userManager.GetUsersCount();

            return Ok(new PagedResult<UserResult>(users, page, count));
        }


        // GET api/users/:id

        /// <summary>
        /// Get the user {id}
        /// </summary>   
        /// <param name="id">The Id of the user</param>        
        /// <response code="200">Returns the user</response>        
        /// <response code="404">User not found</response>
        /// <response code="401">User is denied</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserResult), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult Get(int id)
        {
            try
            {
                var user = userManager.GetUserById(id);
                return Ok(new UserResult(user));
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }


        // GET api/users/current

        /// <summary>
        /// Get the current user
        /// </summary>           
        /// <response code="200">Returns the current user</response>        
        /// <response code="404">User not found</response>
        /// <response code="401">User is denied</response>
        [HttpGet("Current")]
        [ProducesResponseType(typeof(UserResult), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult CurrentUser()
        {
            return Get(User.Identity.GetUserId());
        }
        #endregion

        #region PUT user
        // PUT api/users/current

        /// <summary>
        /// Edit the current user
        /// </summary>   
        /// <param name="model">Contains details of the user to be edited</param>        
        /// <response code="200">User edited successfully</response>        
        /// <response code="400">Request incorrect</response>        
        /// <response code="404">User not found</response>        
        /// <response code="401">User is denied</response>
        [HttpPut("Current")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult Put(UserFormModel model)
        {
            return PutUser(User.Identity.GetUserId(), model);
        }

        // PUT api/users/:id

        /// <summary>
        /// Edit the user {id}
        /// </summary>   
        /// <param name="id">The Id of the user to be edited</param>        
        /// <param name="model">Contains details of the user to be edited</param>        
        /// <response code="200">User edited successfully</response>        
        /// <response code="400">Request incorrect</response>        
        /// <response code="403">User not allowed to edit</response>        
        /// <response code="404">User not found</response>
        /// <response code="401">User is denied</response>
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

        /// <summary>
        /// Get the profile picture of the user {userId}
        /// </summary>   
        /// <param name="userId">Represents the Id of the user</param>
        /// <response code="200">Returns profile picture of the user {userId}</response>        
        /// <response code="404">Resource not found</response>        
        /// <response code="401">User is denied</response>
        [HttpGet("{userId}/picture/")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult GetPictureById(int userId)
        {
            return GetPicture(userId);
        }

        // GET api/users/current/picture/

        /// <summary>
        /// Get the profile picture of the current user
        /// </summary>           
        /// <response code="200">Returns profile picture of the current user</response>        
        /// <response code="404">Resource not found</response>        
        /// <response code="401">User is denied</response>
        [HttpGet("Current/picture/")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult GetPictureForCurrentUser()
        {
            return GetPicture(User.Identity.GetUserId());
        }

        private IActionResult GetPicture(int userId)
        {
            try
            {
                var user = userManager.GetUserById(userId);
                string path = Path.Combine(Constants.ProfilePicturePath, user.Picture);
                if (!System.IO.File.Exists(path))
                    path = Path.Combine(Constants.ProfilePicturePath, Constants.DefaultPircture);

                return Ok(ToBase64String(path));
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        #endregion

        #region POST picture


        // Post api/users/{id}/picture/

        /// <summary>
        /// Add picture for the user {id}
        /// </summary>        
        /// <param name="id">The Id of the user</param>                         
        /// <param name="file">The file to be uploaded</param>                         
        /// <response code="200">Request is accepted</response>        
        /// <response code="400">Request incorrect</response>        
        /// <response code="403">User not allowed to add picture</response>                
        /// <response code="401">User is denied</response>
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

        /// <summary>
        /// Add picture for the current user
        /// </summary>                
        /// <param name="file">The file to be uploaded</param>                         
        /// <response code="200">Request is accepted</response>        
        /// <response code="400">Request incorrect</response>                
        /// <response code="401">User is denied</response>
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
            if (file == null)
                ModelState.AddModelError("file", "File is null");
            else if (file.Length > 1024 * 1024 * 5) // Limit to 5 MB
                ModelState.AddModelError("file", "Picture is to large");
            else if (!IsImage(file))
                ModelState.AddModelError("file", "Invalid Image");
            else
            {
                try
                {
                    var user = userManager.GetUserById(userId);
                    string fileName = user.Id + Path.GetExtension(file.FileName);
                    DeleteFile(Path.Combine(uploads, fileName));

                    using (FileStream outputStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                    {
                        file.CopyTo(outputStream);
                    }

                    userManager.UpdateProfilePicture(user, fileName);
                    return Ok();
                }
                catch (InvalidOperationException)
                {
                    ModelState.AddModelError("userId", "Unkonown User");
                }
            }
            return BadRequest(ModelState);
        }

        private bool IsImage(IFormFile file)
        {
            return ((file != null) && System.Text.RegularExpressions.Regex.IsMatch(file.ContentType, "image/\\S+") && (file.Length > 0));
        }

        #endregion

        #region DELETE picture

        // Delete api/users/:id/picture/

        /// <summary>
        /// Delete picture for the user {id}
        /// </summary>        
        /// <param name="id">The Id of the user</param>                                 
        /// <response code="200">Request is accepted</response>        
        /// <response code="400">Request incorrect</response>        
        /// <response code="403">User not allowed to delete picture</response>                
        /// <response code="401">User is denied</response>
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

        // Delete api/users/current/picture/

        /// <summary>
        /// Delete picture for the current user
        /// </summary>        
        /// <response code="200">Request is accepted</response>        
        /// <response code="400">Request incorrect</response>                
        /// <response code="401">User is denied</response>
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
            try
            {
                var user = userManager.GetUserById(userId);
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
            catch (InvalidOperationException)
            {
                return BadRequest("Could not find User");
            }
        }

        private void DeleteFile(string path)
        {
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
        }

        #endregion
    }

}