using MyTested.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.CmsApi.Controllers;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.User;
using Xunit;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Tests.ControllerTests
{
    public class UserControllerTest
    {
        private ControllerTester<UserController> _tester;

        public UserControllerTest()
        {
            _tester = new ControllerTester<UserController>();
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
                .WithStatusCode(200)
                .WithModelOfType<UserResult>()
                .Passing(m => m.Email == _tester.Admin.Email);                
        }

        /// <summary>
        /// Should return 200 when trying to get user details
        /// </summary>
        [Fact]
        public void GetUserTest200ForStudent()
        {
            _tester.TestController()
                .Calling(c => c.Get(_tester.Student.Email))
                .ShouldReturn()
                .Ok()
                .WithStatusCode(200)
                .WithModelOfType<UserResult>()
                .Passing(m => m.Email == _tester.Student.Email);
        }

        /// <summary>
        /// Should return 404 when trying to get user details
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
    }
}