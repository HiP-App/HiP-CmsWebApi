using PaderbornUniversity.SILab.Hip.CmsApi.Controllers;
using PaderbornUniversity.SILab.Hip.CmsApi.Models;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity;
using MyTested.AspNetCore.Mvc;
using Xunit;
using System.Linq;
using PaderbornUniversity.SILab.Hip.CmsApi.Utility;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.User;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Tests.ControllerTests
{
    public class UsersControllerTest
    {
        private ControllerTester<UsersController> _tester;
        public InviteFormModel InviteFormModel { get; set; }

        public UsersControllerTest()
        {
            _tester = new ControllerTester<UsersController>();
            InviteFormModel = new InviteFormModel()
            {
                Emails = new string[]
                        {
                            "abc@xyz.com", "lmn@xyz.com"
                        }
            };
        }

        #region InviteUsers

        /// <summary>
        /// Should return code 202 when an admin is trying to invite new users with new Emails
        /// </summary>
        //[Fact] - Dependency Injection is not working with XUnit and the issue is still open
        // Refer: https://github.com/xunit/xunit/issues/687
        public void InviteUsersTest202()
        {
            _tester.TestController()
                .Calling(c => c.InviteUsers(InviteFormModel, null))
                .ShouldHave()
                .DbContext(db => db.WithSet<User>(newuser => newuser.Any(actual => actual.Equals(InviteFormModel))))
                .AndAlso()
                .ShouldReturn()
                .StatusCode(202);
        }

        /// <summary>
        /// Should return code 409 when an admin is trying to invite existing users
        /// </summary>
        [Fact]
        public void InviteUsersTest409()
        {
            var student1 = new User
            {
                Id = 4,
                Email = InviteFormModel.Emails[0],
                Role = "Student"
            };

            var student2 = new User
            {
                Id = 5,
                Email = InviteFormModel.Emails[1],
                Role = "Student"
            };

            _tester.TestController()
                .WithDbContext(dbContext => dbContext.WithSet<User>(o => o.AddRange(student1, student2)))
                .Calling(c => c.InviteUsers(InviteFormModel, null)) //Since dependency injection isn't working, EmailSender is null
                .ShouldHave()
                .DbContext(db => db.WithSet<User>(existinguser => existinguser.Any(actual => actual.Equals(student1))))
                .AndAlso()
                .ShouldReturn()
                .StatusCode(409); //user already exists in Database
        }

        /// <summary>
        /// Should return code 403 when a student is trying to invite new users
        /// </summary>
        [Fact]
        public void InviteUsersTest403()
        {
            _tester.TestController(_tester.Student.Email) // --> log in as student
                .Calling(c => c.InviteUsers(InviteFormModel, null)) //Since dependency injection isn't working, EmailSender is null
                .ShouldHave()
                .DbContext(db => db.WithSet<User>(nouser => !nouser.Any(actual => actual.Equals(InviteFormModel))))
                .AndAlso()
                .ShouldReturn()
                .StatusCode(403);
        }

        /// <summary>
        /// Should return code 400 when an admin is trying to invoke with empty Emails
        /// </summary>
        [Fact]
        public void InviteUsersTest400()
        {
            var model = new InviteFormModel()
            {
                Emails = new string[] { }
            };

            _tester.TestController()
                .Calling(c => c.InviteUsers(model, null)) //Since dependency injection isn't working, EmailSender is null                                
                .ShouldReturn()
                .BadRequest()
                .WithStatusCode(400); //As the Emails is empty
        }

        /// <summary>
        /// Should return code 400 when tried to invoke without Emails parameter
        /// </summary>
        [Fact]
        public void InviteUsersTest400WhenEmailParameterIsNotThere()
        {
            var model = new InviteFormModel() { };

            _tester.TestController()
                .Calling(c => c.InviteUsers(model, null)) //Since dependency injection isn't working, EmailSender is null                                
                .ShouldReturn()
                .BadRequest()
                .WithStatusCode(400); //As the Emails is missing
        }

        #endregion

        #region InviteUsers

        /// <summary>
        /// Tests response from Get action of UsersController
        /// </summary>
        [Fact]
        public void GetUserListTest()
        {
            _tester.TestController()
                .Calling(c => c.Get(null, null, 0, 0))                
                .ShouldReturn()
                .Ok()
                .WithModelOfType<PagedResult<UserResult>>()
                .Passing(m => m.Metadata.ItemsCount == 3);
        }

        // <summary>
        /// Tests response from Get action of UsersController
        /// </summary>
        [Fact]
        public void GetUserListTestReturnsOnlyAdmin()
        {
            _tester.TestController()
                .Calling(c => c.Get("Administrator", null, 0, 0))                
                .ShouldReturn()
                .Ok()
                .WithModelOfType<PagedResult<UserResult>>()
                .Passing(m => m.Metadata.ItemsCount == 1);
        }

        #endregion
    }
}
