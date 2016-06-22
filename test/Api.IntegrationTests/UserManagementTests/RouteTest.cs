using Api.Controllers;
using MyTested.AspNetCore.Mvc;
using Xunit;

namespace Api.IntegrationTests.UserManagementTests
{
    public class RouteTest
    {
        /// <summary>
        /// This method is to test whether controllers are found correctly when refered from external assemblies.
        /// </summary>
        [Fact]
        public void UsersIndexShouldMatchCorrectController()
        {
            MyMvc
                .Routes()
                .ShouldMap("/")
                .ToNonExistingRoute();

            /* To Test for Controller Route Use the below
             * const int page = 1;
             * MyMvc
                .Routes()
                .ShouldMap("api/users")
                .To<UsersController>(c => c.Get(null, null, page));
             */
        }
    }
}
