using PaderbornUniversity.SILab.Hip.CmsApi.Utility;
using Microsoft.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.User;
using System;
using Microsoft.Extensions.Logging;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Controllers
{
    public partial class UserController
    {
        /// <summary>
        /// Edit the Student {id}
        /// </summary>         
        /// <param name="model">Contains details of the user to be edited</param>        
        /// <param name="identity">The identity of the user to be edited (For admins)</param>  
        /// <response code="200">Student edited successfully</response>        
        /// <response code="400">Request incorrect</response>        
        /// <response code="403">Student not allowed to edit</response>        
        /// <response code="404">Student not found</response>
        /// <response code="401">User is denied</response>
        [HttpPut("Student")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult PutStudent([FromBody] StudentFormModel model, [FromQuery] string identity)
        {
            if (identity != null && !_userPermissions.IsAllowedToAdminister(User.Identity.GetUserIdentity()))
                return Forbidden();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var student = _userManager.GetStudentById(identity ?? User.Identity.GetUserIdentity());
                _userManager.PutStudentDetials(student, model);
                Logger.LogInformation(42, "Studet with identity: " + identity + " updated.");
                return Ok();

            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }
    }
}
