﻿using Api.Utility;
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
                if (!user.HasProfilePicture() || Constants.DefaultPircture.Equals(user.Picture))
                    return BadRequest("No picture set");

                bool success = userManager.UpdateProfilePicture(user, null);
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