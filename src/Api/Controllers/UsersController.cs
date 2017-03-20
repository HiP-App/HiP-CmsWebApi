using Api.Data;
using Api.Managers;
using Api.Models;
using Api.Models.User;
using Api.Permission;
using Api.Services;
using Api.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    public class UsersController : ApiController
    {
        private readonly UserManager _userManager;
        private readonly UserPermissions _userPermissions;

        public UsersController(CmsDbContext dbContext, ILoggerFactory loggerFactory) : base(dbContext, loggerFactory)
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
            if (!_userPermissions.IsAllowedToInvite(User.Identity.GetUserIdentity()))
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
        [HttpGet("/Api/Users/")]
        [ProducesResponseType(typeof(PagedResult<UserResult>), 200)]
        public IActionResult Get([FromQuery]string role, [FromQuery]string query, [FromQuery] int page = 0, [FromQuery] int pageSize = Constants.PageSize)
        {
            return Ok(_userManager.GetAllUsers(query, role, page, pageSize));
        }

        #endregion
    }
}
