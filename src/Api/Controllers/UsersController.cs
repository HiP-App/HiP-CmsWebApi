using Api.Utility;
using Microsoft.AspNetCore.Mvc;
using Api.Managers;
using Microsoft.Extensions.Logging;
using Api.Data;
using Api.Models;
using Api.Models.User;
using Api.Permission;
using Api.Services;
using System;
using Api.Models.Entity;

namespace Api.Controllers
{
    public partial class UsersController : ApiController
    {
        private readonly UserManager _userManager;
        private readonly UserPermissions _userPermissions;

        public UsersController(CmsDbContext dbContext, ILoggerFactory logger) : base(dbContext, logger)
        {
            _userManager = new UserManager(dbContext);
            _userPermissions = new UserPermissions(dbContext);
        }

        #region invite

        /// <summary>
        /// Add new users and send invitation to the added users for registration
        /// </summary>        
        /// <param name="model">Contains a list of emails</param>
        /// <param name="emailSender">EmailSender Service</param>
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
            if (!_userPermissions.IsAllowedToInvite(User.Identity.GetUserIdenty()))
                return Forbidden();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _userManager.InviteUsers(model.Emails, emailSender);
            if (result.FailedInvitations.Count == model.Emails.Length)
                return BadRequest(result);

            if (result.ExistingUsers.Count == model.Emails.Length)
                return StatusCode(409, result);

            return Accepted(result);
        }

        #endregion

        #region GET user

        // GET api/users

        /// <summary>
        /// All users matching query and role
        /// </summary>   
        /// <param name="role">Represents role of the user</param>
        /// <param name="query">Users containing query in email, first and last name</param>        
        /// <param name="page">Represents the page</param>
        /// <param name="pageSize">Size of the requested page</param>
        /// <response code="200">Returns PagedResults of UserResults</response>        
        /// <response code="401">User is denied</response>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<UserResult>), 200)]
        public IActionResult Get([FromQuery]string role, [FromQuery]string query, [FromQuery] int page = 0, [FromQuery] int pageSize = Constants.PageSize)
        {
            return Ok(_userManager.GetAllUsers(query, role, page, pageSize));
        }


        // GET api/users/:id

        /// <summary>
        /// Get the user {id}
        /// </summary>   
        /// <param name="identy">The identy of the user </param>       
        /// <response code="200">Returns the user</response>        
        /// <response code="404">User not found</response>
        /// <response code="401">User is denied</response>
        [HttpGet("User")]
        [ProducesResponseType(typeof(UserResult), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult Get([FromQuery] string identy)
        {
            try
            {
                var user = _userManager.GetUserByIdenty(identy ?? User.Identity.GetUserIdenty());
                return Ok(new UserResult(user));
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        #endregion

        #region PUT user

        // PUT api/users

        /// <summary>
        /// Edit the user {id}
        /// </summary> 
        /// <param name="identy">The identy of the user to be edited (For admins)</param>          
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
        public IActionResult Put([FromRoute]int id, [FromBody]AdminUserFormModel model)
        {
            // TODO
            if (!_userPermissions.IsAllowedToAdminister(User.Identity.GetUserIdenty()))
                return Forbidden();

            var user = _userManager.GetUserById(id);

            return PutUser(user, model);
        }

        private IActionResult PutUser(User user, UserFormModel model)
        {
            if (ModelState.IsValid)
            {
                if (model is AdminUserFormModel && !Role.IsRoleValid(((AdminUserFormModel)model).Role))
                {
                    ModelState.AddModelError("Role", "Invalid Role");
                }
                else
                {
                    if (!_userManager.UpdateUser(user.Email, model))
                        return NotFound();

                    Logger.LogInformation(5, "User with ID: " + user.Id + " updated.");
                    return Ok();
                }
            }

            return BadRequest(ModelState);
        }

        #endregion
    }

}