using Api.Controllers;
using Api.ViewModels.Users;
using MyTested.AspNetCore.Mvc;
using Xunit;

namespace Api.UnitTests.Controller
{
    public class UsersControllerTest
    {
        /// <summary>
        /// GetValidUsersTest is a Test method to get valid users.
        /// </summary>
        [Fact]
        public void GetValidUsersTest()
        {
            const int page = 1;
            MyMvc
                .Controller<UsersController>()
                .Calling(c => c.Get(null, null, page))
                .ShouldReturn()
                .Ok();
        }

        /// <summary>
        /// UpdatedUsersTest is a Test method to update the user's role.
        /// </summary>
        [Fact]
        public void UpdatedUsersTest()
        {
            MyMvc
                .Controller<UsersController>()
                .Calling(c => c.Put(4, new ChangeRoleModel
                {
                    Role = "AnonymousRole"
                }))
                .ShouldReturn()
                .BadRequest();
        }
    }
}
