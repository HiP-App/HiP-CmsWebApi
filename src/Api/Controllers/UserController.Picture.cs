using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.User;
using PaderbornUniversity.SILab.Hip.CmsApi.Utility;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Controllers
{
    public partial class UserController
    {

        #region GET picture

        /// <summary>
        /// Get the profile picture 
        /// </summary>       
        /// <param name="identity">Specify the identity</param>    
        /// <response code="200">Returns profile picture of the current user</response>        
        /// <response code="404">Resource not found</response>        
        /// <response code="401">User is denied</response>
        [HttpGet("Picture/")]
        [ProducesResponseType(typeof(Base64Image), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult GetPictureByIdentity([FromQuery]string identity)
        {
            try
            {
                var user = _userManager.GetUserByEmail(identity ?? User.Identity.GetUserIdentity());
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


        // Post api/users/picture/

        /// <summary>
        /// Add picture for the user
        /// </summary>        
        /// <param name="identity">The identity of the user to be edited (For admins)</param>                      
        /// <param name="file">The file to be uploaded</param>                         
        /// <response code="200">Request is accepted</response>        
        /// <response code="400">Request incorrect</response>        
        /// <response code="403">User not allowed to add picture</response>                
        /// <response code="401">User is denied</response>
        [HttpPut("Picture/")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 403)]
        public IActionResult PutPicture([FromQuery] string identity, [FromForm] IFormFile file)
        {
            if (identity != null && !_userPermissions.IsAllowedToAdminister(User.Identity))
                return Forbidden();

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
                    var user = _userManager.GetUserByEmail(identity ?? User.Identity.GetUserIdentity());
                    var fileName = user.Id + Path.GetExtension(file.FileName);
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
            return ((file != null) && Regex.IsMatch(file.ContentType, "image/\\S+") && (file.Length > 0));
        }

        #endregion

        #region DELETE picture

        // Delete api/users/:id/picture/

        /// <summary>
        /// Delete picture for the current user
        /// </summary>        
        /// <param name="identity">The identity of the user to be delete (For admins)</param>                                 
        /// <response code="200">Request is accepted</response>        
        /// <response code="400">Request incorrect</response>        
        /// <response code="403">User not allowed to delete picture</response>                
        /// <response code="401">User is denied</response>
        [HttpDelete("Picture")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 403)]
        public IActionResult Delete([FromQuery] string identity)
        {
            if (identity != null && !_userPermissions.IsAllowedToAdminister(User.Identity))
                return Forbidden();
            // Fetch user
            try
            {
                var user = _userManager.GetUserByEmail(identity ?? User.Identity.GetUserIdentity());
                // Has A Picture?
                if (string.IsNullOrEmpty(user.ProfilePicture) || Constants.DefaultPircture.Equals(user.ProfilePicture))
                    return BadRequest("No picture set");

                // Delete Picture If Exists
                var fileName = Path.Combine(Constants.ProfilePicturePath, user.ProfilePicture);

                DeleteFile(fileName);

                if (_userManager.UpdateProfilePicture(user, null))
                    return Ok();
                return BadRequest();
            }
            catch (InvalidOperationException)
            {
                return BadRequest("Could not find User");
            }
        }

        private static void DeleteFile(string path)
        {
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
        }

        #endregion
    }

}