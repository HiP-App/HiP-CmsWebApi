using Api.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using Api.Models.User;
using System;

namespace Api.Controllers
{
    public partial class UsersController
    {

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
        [ProducesResponseType(typeof(Base64Image), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult GetPictureById([FromRoute]int userId)
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
        [ProducesResponseType(typeof(Base64Image), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult GetPictureForCurrentUser()
        {
            return GetPicture(User.Identity.GetUserId());
        }

        private IActionResult GetPicture([FromRoute]int userId)
        {
            try
            {
                var user = _userManager.GetUserById(userId);
                var path = Path.Combine(Constants.ProfilePicturePath, user.Picture);
                if (!System.IO.File.Exists(path))
                    path = Path.Combine(Constants.ProfilePicturePath, Constants.DefaultPircture);

                return Ok(new Base64Image(ToBase64String(path)));
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        #endregion

        #region PUT picture


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
        [HttpPut("{id}/picture/")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 403)]
        public IActionResult PutPicture([FromRoute]int id, [FromForm] IFormFile file)
        {
            if (!_userPermissions.IsAllowedToAdminister(User.Identity.GetUserId()))
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
        [HttpPut("Current/picture/")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult PutPicture([FromForm]IFormFile file)
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
                    var user = _userManager.GetUserById(userId);
                    string fileName = user.Id + Path.GetExtension(file.FileName);
                    DeleteFile(Path.Combine(uploads, fileName));

                    using (FileStream outputStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                    {
                        file.CopyTo(outputStream);
                    }

                    _userManager.UpdateProfilePicture(user, fileName);
                    return Ok();
                }
                catch (InvalidOperationException)
                {
                    ModelState.AddModelError("userId", "Unkonown User");
                }
            }
            return BadRequest(ModelState);
        }

        private static bool IsImage(IFormFile file)
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
        public IActionResult Delete([FromRoute]int id)
        {
            if (!_userPermissions.IsAllowedToAdminister(User.Identity.GetUserId()))
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

        private IActionResult DeletePicture([FromRoute]int userId)
        {
            // Fetch user
            try
            {
                var user = _userManager.GetUserById(userId);
                // Has A Picture?
                if (string.IsNullOrEmpty(user.ProfilePicture) || Constants.DefaultPircture.Equals(user.ProfilePicture))
                    return BadRequest("No picture set");

                var success = _userManager.UpdateProfilePicture(user, null);
                // Delete Picture If Exists
                var fileName = Path.Combine(Constants.ProfilePicturePath, user.Picture);

                DeleteFile(fileName);

                if (success)
                    return Ok();
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