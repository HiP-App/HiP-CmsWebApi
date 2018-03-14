using Microsoft.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.CmsApi.Managers;
using Microsoft.Extensions.Logging;
using PaderbornUniversity.SILab.Hip.CmsApi.Data;
using PaderbornUniversity.SILab.Hip.CmsApi.Models;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.User;
using PaderbornUniversity.SILab.Hip.CmsApi.Permission;
using System;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity;
using PaderbornUniversity.SILab.Hip.CmsApi.Utility;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Controllers
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
        /// <param name="identity">The identity of the user </param>       
        /// <response code="200">Returns the user</response>        
        /// <response code="404">User not found</response>
        /// <response code="401">User is denied</response>
        [HttpGet]
        [ProducesResponseType(typeof(UserResult), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult Get([FromQuery] string identity)
        {
            try
            {
                var uid = identity ?? User.Identity.GetUserIdentity();
                var user = _userManager.GetUserByIdentity(uid);
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
        /// <param name="identity">The identity of the user to be edited (For admins)</param>          
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
        public IActionResult Put([FromQuery]string identity, [FromBody]UserFormModel model)
        {
            if (identity != null && !_userPermissions.IsAllowedToAdminister(User.Identity))
                return Forbidden();

            if (identity != null && model.Role != null && !Role.IsRoleValid(model.Role))
                ModelState.AddModelError("Role", "Invalid Role");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                User user;
                if (identity == null)
                {
                    user = _userManager.GetUserByIdentity(User.Identity.GetUserIdentity());
                }
                else
                {
                    user = _userManager.GetUserByIdentity(identity);
                }
                _userManager.UpdateUser(user, model, (identity != null && model.Role != null));
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