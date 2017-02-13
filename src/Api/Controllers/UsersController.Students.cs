using Api.Utility;
using Microsoft.AspNetCore.Mvc;
using Api.Models.User;
using System;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    public partial class UsersController
    {
        /// <summary>
        /// Edit the current Student
        /// </summary>   
        /// <param name="model">Contains details of the user to be edited</param>        
        /// <response code="200">Student edited successfully</response>        
        /// <response code="400">Request incorrect</response>        
        /// <response code="404">Student not found</response>        
        /// <response code="401">User is denied</response>
        [HttpPut("Student/Current")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult PutStudent([FromBody] StudentFormModel model)
        {
            return PutStudentDetials(User.Identity.GetUserId(), model);
        }

        /// <summary>
        /// Edit the Student {id}
        /// </summary>   
        /// <param name="id">The Id of the user to be edited</param>        
        /// <param name="model">Contains details of the user to be edited</param>        
        /// <response code="200">Student edited successfully</response>        
        /// <response code="400">Request incorrect</response>        
        /// <response code="403">Student not allowed to edit</response>        
        /// <response code="404">Student not found</response>
        /// <response code="401">User is denied</response>
        [HttpPut("Student/{id}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult PutStudent([FromRoute] int id, [FromBody] StudentFormModel model)
        {
            if (!_userPermissions.IsAllowedToAdminister(User.Identity.GetUserId()))
                return Forbidden();

            return PutStudentDetials(id, model);
        }


        private IActionResult PutStudentDetials(int id, StudentFormModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var student = _userManager.GeStudentById(id);
                _userManager.PutStudentDetials(student, model);
                Logger.LogInformation(42, "Studet with ID: " + id + " updated.");
                return Ok();

            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }
    }
}
