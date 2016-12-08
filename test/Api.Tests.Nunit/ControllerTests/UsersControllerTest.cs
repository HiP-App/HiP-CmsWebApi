using Api.Controllers;
using Api.Models;
using MyTested.AspNetCore.Mvc;
using NUnit.Framework;

namespace Api.Tests.Nunit.ControllerTests
{
    [TestFixture]
    public class UsersControllerTest
    {
        /// <summary>
        /// Tests response from Get action of UsersController.
        /// </summary>
        [Test]
        public void GetUserListTest()
        {
            MyMvc
                .Controller<UsersController>()
                .Calling(c => c.Get(null, null, 1))
                .ShouldReturn()
                .Ok();
        }

        /// <summary>
        /// Tests response code 403 from Put(Id,AdminUserFormModel) action of UsersController
        /// </summary>
        [Test]
        public void UpdateUserTestResponseCode403()
        {
            MyMvc
                .Controller<UsersController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1")) // add claim with custom name and value of 1                                
                .Calling(c => c.Put(4, new AdminUserFormModel
                {
                    FirstName = "First Name",
                    LastName = "Last Name",
                    Role = "InvalidRole"
                }))                
                .ShouldReturn()
                .StatusCode(403);
        }

        /// <summary>
        /// Tests response from Put(Id,AdminUserFormModel) action of UsersController
        /// </summary>
        [Test]
        public void UpdateUserTest()
        {
            MyMvc
                .Controller<UsersController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1")) // add claim with custom name and value of 1                                
                .Calling(c => c.Put(4, new AdminUserFormModel
                {
                    FirstName = "First Name",
                    LastName = "Last Name",
                    Role = "InvalidRole"
                }))
                .ShouldReturn()
                .BadRequest();
        }
    }
}
