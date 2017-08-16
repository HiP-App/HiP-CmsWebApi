using MyTested.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.CmsApi.Controllers;
using PaderbornUniversity.SILab.Hip.CmsApi.Models;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.User;
using Xunit;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Tests.ControllerTests
{
    public class UserControllerTest
    {
        private ControllerTester<UserController> _tester;
        public UserFormModel UserFormModel { get; set; }

        public UserControllerTest()
        {
            _tester = new ControllerTester<UserController>();
            UserFormModel = new UserFormModel
            {
                FirstName = "First Name",
                LastName = "Last Name"
            };
        }

        #region Get User

        /// <summary>
        /// Should return 200 when trying to get user details
        /// </summary>
        [Fact]
        public void GetUserTest200()
        {
            _tester.TestController()
                .Calling(c => c.Get(_tester.Admin.Email))
                .ShouldReturn()
                .Ok()                
                .WithModelOfType<UserResult>()
                .Passing(m => m.Email == _tester.Admin.Email);                
        }

        /// <summary>
        /// Should return 404 when trying to get a non-existing user
        /// </summary>
        [Fact]
        public void GetUserTest404()
        {
            _tester.TestController()
                .Calling(c => c.Get("hello@hipapp.de"))
                .ShouldReturn()
                .NotFound();
        }

        #endregion

        #region PUT user tests

        /// <summary>
        /// Should return code 200 if user is updated
        /// </summary>
        [Fact]
        public void PutUserTest200()
        {
            _tester.TestController()
                .Calling(c => c.Put(_tester.Admin.Email, UserFormModel))
                .ShouldHave()
                .DbContext(db => db.WithSet<User>(user =>
                    user.Find(_tester.Admin.Id).FirstName.Equals(UserFormModel.FirstName) &&
                    user.Find(_tester.Admin.Id).FirstName.Equals(UserFormModel.FirstName)
                ))
                .AndAlso()
                .ShouldReturn()
                .Ok();
        }

        /// <summary>
        /// Should return bad request when trying to update user with invalid role
        /// </summary>
        [Fact]
        public void PutUserTest400()
        {
            var userFormModel = new UserFormModel
            {
                FirstName = "First Name",
                LastName = "Last Name",
                Role = "InvalidRole"
            };

            _tester.TestController()
                .Calling(c => c.Put(_tester.Admin.Email, userFormModel))
                .ShouldReturn()
                .BadRequest()
                .WithStatusCode(400);
        }

        /// <summary>
        /// Should return 403 when student trying to update user
        /// </summary>
        [Fact]
        public void PutUserTest403()
        {
            _tester.TestController(_tester.Student.UId, "Student")
                .Calling(c => c.Put(_tester.Admin.Email, UserFormModel))
                .ShouldReturn()
                .StatusCode(403);
        }

        /// <summary>
        /// Should return user not found when a new user is trying to get updated
        /// </summary>
        [Fact]
        public void PutUserTest404()
        {
            _tester.TestController()
                .Calling(c => c.Put("nonexistinguser@hipapp.de", UserFormModel))
                .ShouldReturn()
                .NotFound()
                .WithStatusCode(404);
        }

        #endregion

    }
}