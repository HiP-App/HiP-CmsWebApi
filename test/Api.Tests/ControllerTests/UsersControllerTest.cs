using Api.Controllers;
using Api.ViewModels.Users;
using MyTested.AspNetCore.Mvc;
using Xunit;

namespace Api.Tests.ControllerTests
{
    public class UsersControllerTest
    {
        /// <summary>
        /// Tests response from Get action of UsersController.
        /// </summary>
        [Fact]
        public void GetUserListTest()
        {
            MyMvc
                .Controller<UsersController>()
                .Calling(c => c.Get(null, null, 1))
                .ShouldReturn()
                .Ok();
        }

        /// <summary>
        /// Tests response from Put(id) action of UsersController
        /// </summary>
        [Fact]
        public void UpdateUserRoleTest()
        {
            MyMvc
                .Controller<UsersController>()
                .Calling(c => c.Put(4, new ChangeRoleModel
                {
                    Role = "InvalidRole"
                }))
                .ShouldReturn()
                .BadRequest();
        }
    }
}
