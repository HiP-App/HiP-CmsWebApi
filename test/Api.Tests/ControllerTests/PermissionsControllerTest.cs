﻿using MyTested.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.CmsApi.Controllers;
using PaderbornUniversity.SILab.Hip.UserStore;
using System.Collections.ObjectModel;
using Xunit;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Tests.ControllerTests
{
    public class PermissionControllerTest
    {
        private ControllerTester<PermissionsController> _tester;

        public PermissionControllerTest()
        {
            _tester = new ControllerTester<PermissionsController>();
        }

        #region Annotations test

        /// <summary>
        /// Should return ok if admin tries to create tags
        /// </summary>
        [Fact]
        public void IsAllowedToCreateTagsTestOk()
        {
            _tester.TestController()
                .Calling(c => c.IsAllowedToCreateTagsAsync())
                .ShouldReturn()
                .Ok();
        }

        /// <summary>
        /// Should return Unauthorized if student tries to create tags
        /// </summary>
        [Fact]
        public void IsAllowedToCreateTagsTestUnauthorized()
        {
            _tester.TestController(_tester.Student.Id)
                .Calling(c => c.IsAllowedToCreateTagsAsync())
                .ShouldReturn()
                .Unauthorized();
        }

        /// <summary>
        /// Should return ok if admin tries to edit tags
        /// </summary>
        [Fact]
        public void IsAllowedToEditTagsTestOk()
        {
            _tester.TestController()
                .Calling(c => c.IsAllowedToEditTagsAsync())
                .ShouldReturn()
                .Ok();
        }

        /// <summary>
        /// Should return Unauthorized if student tries to edit tags
        /// </summary>
        [Fact]
        public void IsAllowedToEditTagsTestUnauthorized()
        {
            _tester.TestController(_tester.Student.Id)
                .Calling(c => c.IsAllowedToEditTagsAsync())
                .ShouldReturn()
                .Unauthorized();
        }

        #endregion

        #region Topics test

        /// <summary>
        /// Should return ok if admin tries to create topics
        /// </summary>
        [Fact]
        public void IsAllowedToCreateTestOk()
        {
            _tester.TestController()
                .Calling(c => c.IsAllowedToCreateAsync())
                .ShouldReturn()
                .Ok();
        }

        /// <summary>
        /// Should return status code 403 if student tries to create topics
        /// </summary>
        [Fact]
        public void IsAllowedToCreateTestForbidden()
        {
            _tester.TestController(_tester.Student.Id)
                .Calling(c => c.IsAllowedToCreateAsync())
                .ShouldReturn()
                .StatusCode(403); // Not able to use Forbidden() as the original method returns the status code only but not Forbid()
        }

        /// <summary>
        /// Should return ok if admin tries to edit topics
        /// </summary>
        [Fact]
        public void IsAllowedToEditTestOk()
        {
            _tester.TestController()
                .Calling(c => c.IsAllowedToEditAsync(_tester.TopicOne.Id))
                .ShouldReturn()
                .Ok();
        }

        /// <summary>
        /// Should return status code 403 if student tries to edit topics
        /// </summary>
        [Fact]
        public void IsAllowedToEditTestForbidden()
        {
            _tester.TestController(_tester.Student.Id)
                .Calling(c => c.IsAllowedToEditAsync(_tester.TopicOne.Id))
                .ShouldReturn()
                .StatusCode(403);
        }

        /// <summary>
        /// Should return ok if admin tries to associate with the topic
        /// </summary>
        [Fact]
        public void IsAssociatedToTestOk()
        {
            _tester.TestControllerWithMockData()
                .Calling(c => c.IsAssociatedToAsync(_tester.TopicOne.Id))
                .ShouldReturn()
                .Ok();
        }

        /// <summary>
        /// Should return ok if supervisor has created a topic and he tries to associate with the topic
        /// </summary>
        [Fact]
        public void IsAssociatedToTestForSupervisor()
        {
            _tester.TestControllerWithMockData(_tester.Supervisor.Id)
                .Calling(c => c.IsAssociatedToAsync(_tester.TopicOne.Id))
                .ShouldReturn()
                .Ok();
        }

        /// <summary>
        /// Should return ok if student tries to associate with the topic
        /// </summary>
        [Fact]
        public void IsAssociatedToTestForStudent()
        {
            _tester.TestControllerWithMockData(_tester.Student.Id)
                .Calling(c => c.IsAssociatedToAsync(_tester.TopicOne.Id))
                .ShouldReturn()
                .Ok();
        }

        /// <summary>
        /// Should return code 403 if a new student tries to associate with the topic
        /// </summary>
        [Fact]
        public void IsAssociatedToTestForbidden()
        {
            var newStudent = new UserResult
            {
                Id = "test-auth:student13",
                Email = "student1@hipapp.de",
                Roles = new ObservableCollection<string>(new[] { "Student" })
            };

            _tester.TestControllerWithMockData(newStudent.Id)
                .WithDbContext(dbContext => { })
                .Calling(c => c.IsAssociatedToAsync(_tester.TopicOne.Id))
                .ShouldReturn()
                .StatusCode(403);
        }

        /// <summary>
        /// Should return ok if reviewer tries to associate with the topic
        /// </summary>
        [Fact]
        public void IsAllowedToReviewTest()
        {
            _tester.TestControllerWithMockData(_tester.Reviewer.Id)
                .Calling(c => c.IsAllowedToReviewAsync(_tester.TopicOne.Id))
                .ShouldReturn()
                .Ok();
        }

        /// <summary>
        /// Should return code 403 if admin tries to associate with the topic
        /// </summary>
        [Fact]
        public void IsAllowedToReviewTest403ForAdmin()
        {
            _tester.TestControllerWithMockData()
                .Calling(c => c.IsAllowedToReviewAsync(_tester.TopicOne.Id))
                .ShouldReturn()
                .StatusCode(403);
        }

        /// <summary>
        /// Should return code 403 if student tries to associate with the topic
        /// </summary>
        [Fact]
        public void IsAllowedToReviewTest403ForStudent()
        {
            _tester.TestControllerWithMockData(_tester.Student.Id)
                .Calling(c => c.IsAllowedToReviewAsync(_tester.TopicOne.Id))
                .ShouldReturn()
                .StatusCode(403);
        }

        #endregion

        #region User test

        /// <summary>
        /// Should return ok if admin tries to administer
        /// </summary>
        [Fact]
        public void IsAllowedToAdministerTestOk()
        {
            _tester.TestController()
                .Calling(c => c.IsAllowedToAdminister())
                .ShouldReturn()
                .Ok();
        }

        /// <summary>
        /// Should return code 403 if student tries to administer
        /// </summary>
        [Fact]
        public void IsAllowedToAdministerTestForbiddenForStudent()
        {
            _tester.TestController(_tester.Student.Id, "Student")
                .Calling(c => c.IsAllowedToAdminister())
                .ShouldReturn()
                .StatusCode(403);
        }

        /// <summary>
        /// Should return code 403 if supervisor tries to administer
        /// </summary>
        [Fact]
        public void IsAllowedToAdministerTestForbiddenForSupervisor()
        {
            _tester.TestController(_tester.Supervisor.Id, "Supervisor")
                .Calling(c => c.IsAllowedToAdminister())
                .ShouldReturn()
                .StatusCode(403);
        }


        /// <summary>
        /// Should return ok if admin tries to invite users
        /// </summary>
        [Fact]
        public void IsAllowedToInviteTestOk()
        {
            _tester.TestController()
                .Calling(c => c.IsAllowedToInvite())
                .ShouldReturn()
                .Ok();
        }

        /// <summary>
        /// Should return code 403 if student tries to invite users
        /// </summary>
        [Fact]
        public void IsAllowedToInviteForbiddenForStudent()
        {
            _tester.TestController(_tester.Student.Id, "Student")
                .Calling(c => c.IsAllowedToInvite())
                .ShouldReturn()
                .StatusCode(403);
        }

        /// <summary>
        /// Should return ok if supervisor tries to invite users
        /// </summary>
        [Fact]
        public void IsAllowedToInviteTestForSupervisor()
        {
            _tester.TestController(_tester.Supervisor.Id)
                .Calling(c => c.IsAllowedToInvite())
                .ShouldReturn()
                .Ok();
        }
        #endregion

    }
}