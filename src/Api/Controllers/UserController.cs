using Microsoft.AspNetCore.Mvc;
using Api.Managers;
using Microsoft.Extensions.Logging;
using Api.Data;
using Api.Models;
using Api.Models.User;
using Api.Permission;
using System;
using Api.Utility;

namespace Api.Controllers
{
    public partial class UserController : ApiController
    {
        private readonly UserManager _userManager;
        private readonly UserPermissions _userPermissions;

        public UserController(CmsDbContext dbContext, ILoggerFactory logger) : base(dbContext, logger)
        {
            _userManager = new UserManager(dbContext);
            _userPermissions = new UserPermissions(dbContext);
        }



        #region GET user

        /// <summary>
        /// Get the user
        /// </summary>   
        /// <param name="identy">The identy of the user </param>       
        /// <response code="200">Returns the user</response>        
        /// <response code="404">User not found</response>
        /// <response code="401">User is denied</response>
        [HttpGet]
        [ProducesResponseType(typeof(UserResult), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult Get([FromQuery] string identy)
        {
            try
            {
                var user = _userManager.GetUserByIdentity(identy ?? User.Identity.GetUserIdentity());
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
        /// Edit the user 
        /// </summary> 
        /// <param name="identy">The identy of the user to be edited (For admins)</param>          
        /// <param name="model">Contains details of the user to be edited</param>        
        /// <response code="200">User edited successfully</response>        
        /// <response code="400">Request incorrect</response>        
        /// <response code="403">User not allowed to edit</response>        
        /// <response code="404">User not found</response>
        /// <response code="401">User is denied</response>
        [HttpPut]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult Put([FromQuery]string identy, [FromBody]UserFormModel model)
        {
            if (identy != null && !_userPermissions.IsAllowedToAdminister(User.Identity.GetUserIdentity()))
                return Forbidden();

            if (identy != null && model.Role != null && !Role.IsRoleValid(model.Role))
                ModelState.AddModelError("Role", "Invalid Role");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = _userManager.GetUserByIdentity(identy ?? User.Identity.GetUserIdentity());
                _userManager.UpdateUser(user, model, (identy != null && model.Role != null));
                Logger.LogInformation(5, "User with ID: " + user.Id + " updated.");
                return Ok();
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        #endregion
    }

}