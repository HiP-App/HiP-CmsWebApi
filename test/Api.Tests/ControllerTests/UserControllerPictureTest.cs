using MyTested.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.CmsApi.Controllers;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.User;
using Xunit;
namespace PaderbornUniversity.SILab.Hip.CmsApi.Tests.ControllerTests
{
    public class UserControllerPictureTest
    {
        private ControllerTester<UserController> _tester;

        public UserControllerPictureTest()
        {
            _tester = new ControllerTester<UserController>();
        }
        
        #region Get Picture
        
        /// <summary>
        /// Should return 200 when trying to get user details
        /// </summary>
        [Fact]
        public void GetPictureByIdentityTest()
        {
            _tester.TestControllerWithMockData()
                .Calling(c => c.GetPictureByIdentity(_tester.Admin.Email))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<Base64Image>()
                .Passing(m => m.Base64 != null );
        }

        /// <summary>
        /// Should return 404 when the user is not found to retrieve the profile picture
        /// </summary>
        [Fact]
        public void GetPictureByIdentityTest404()
        {
            _tester.TestControllerWithMockData()
                .Calling(c => c.GetPictureByIdentity("newuser@hipapp.de"))
                .ShouldReturn()
                .NotFound();
        }

        #endregion

        #region Put Picture

        /// <summary>
        /// Should return bad request when trying to update the profile picture with no file attached
        /// </summary>
        [Fact]
        public void PutPictureTestBadRequestWhenFileIsNull()
        {
            _tester.TestControllerWithMockData()
                .Calling(c => c.PutPicture(_tester.Admin.Email, null)) // Since file cannot be attached, it is kept null
                .ShouldReturn()
                .BadRequest();
        }

        /// <summary>
        /// Should return 403 when the user doesn't have permission to update the profile picture
        /// </summary>
        [Fact]
        public void PutPictureTest403()
        {
            _tester.TestControllerWithMockData("newuser@hipapp.de")
                .Calling(c => c.PutPicture(_tester.Admin.Email, null)) // Since file cannot be attached, it is kept null
                .ShouldReturn()
                .StatusCode(403);
        }

        #endregion

        #region Delete Picture

        /// <summary>
        /// Should return bad request when trying to delete the profile picture when he has only the default picture
        /// </summary>
        [Fact]
        public void DeleteTest200()
        {
            _tester.TestControllerWithMockData()
                .Calling(c => c.Delete(_tester.Admin.Email))
                .ShouldReturn()
                .BadRequest(); // As the user has a default picture
        }

        /// <summary>
        /// Should return 403 when the user doesn't have permission to update the profile picture
        /// </summary>
        [Fact]
        public void DeleteTest403()
        {
            _tester.TestControllerWithMockData("newuser@hipapp.de")
                .Calling(c => c.Delete(_tester.Admin.Email)) 
                .ShouldReturn()
                .StatusCode(403);
        }

        #endregion
    }
}