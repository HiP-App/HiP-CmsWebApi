using Api.Utility;
using Microsoft.AspNetCore.Mvc;
using Api.Managers;
using Microsoft.Extensions.Logging;
using Api.Data;
using Api.Models;
using Api.Models.User;
using Api.Permission;
using System;

namespace Api.Controllers
{
    public partial class UsersController : ApiController
    {
        private readonly UserManager _userManager;
        private readonly IEmailSender _emailSender;
        private readonly UserPermissions _userPermissions;

        public UsersController(CmsDbContext dbContext, IEmailSender emailSender, ILoggerFactory logger) : base(dbContext, logger)
        {
            _userManager = new UserManager(dbContext);
            _userPermissions = new UserPermissions(dbContext);
            _emailSender = emailSender;
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
        [ProducesResponseType(typeof(void), 202)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 409)]
        [ProducesResponseType(typeof(void), 503)]
        public IActionResult Post([FromBody]InviteFormModel model)
        {
            if (!_userPermissions.IsAllowedToInvite(User.Identity.GetUserId()))
                return Forbidden();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var failCount = 0;
            foreach (var email in model.emails)
            {
                try
                {
                    _userManager.AddUserbyEmail(email);
                    _emailSender.InviteAsync(email);
                }
                //user already exists in Database
                catch (Microsoft.EntityFrameworkCore.DbUpdateException)
                {
                    failCount++;
                }
                //something went wrong when sending email
                catch (MailKit.Net.Smtp.SmtpCommandException smtpError)
                {
                    Logger.LogDebug(smtpError.ToString());
                    return ServiceUnavailable();
                }
            }
            if (failCount == model.emails.Length)
                return Conflict();

            return Accepted();
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
        public IActionResult Get([FromQuery]string query, [FromQuery]string role, [FromQuery] int page = 1)
        {
            var users = _userManager.GetAllUsers(query, role, page, Constants.PageSize);
            int count = _userManager.GetUsersCount();

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
        public IActionResult Get([FromRoute]int id)
        {
            try
            {
                var user = _userManager.GetUserById(id);
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
        public IActionResult Put([FromBody]UserFormModel model)
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
        public IActionResult Put([FromRoute]int id, [FromBody]AdminUserFormModel model)
        {
            if (!_userPermissions.IsAllowedToAdminister(User.Identity.GetUserId()))
                return Forbidden();

            return PutUser(id, model);
        }

        private IActionResult PutUser([FromRoute]int id, [FromBody]UserFormModel model)
        {
            if (ModelState.IsValid)
            {
                if (model is AdminUserFormModel && !Role.IsRoleValid(((AdminUserFormModel)model).Role))
                {
                    ModelState.AddModelError("Role", "Invalid Role");
                }
                else
                {
                    if (_userManager.UpdateUser(id, model))
                    {
                        Logger.LogInformation(5, "User with ID: " + id + " updated.");
                        return Ok();
                    }
                    return NotFound();
                }
            }

            return BadRequest(ModelState);
        }

        #endregion
    }

}