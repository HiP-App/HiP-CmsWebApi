using MyTested.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.CmsApi.Controllers;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.User;
using PaderbornUniversity.SILab.Hip.UserStore;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Tests.ControllerTests
{
    public class TopicsControllerUsersTest
    {
        private ControllerTester<TopicsController> _tester;
        public UserResult Student1 { get; set; }
        public UserResult Student2 { get; set; }
        public UserResult Supervisor1 { get; set; }
        public UserResult Supervisor2 { get; set; }
        public UserResult Reviewer1 { get; set; }
        public UserResult Reviewer2 { get; set; }
        public UsersFormModel UsersFormModelForStudent { get; set; }
        public UsersFormModel UsersFormModelForSupervisor { get; set; }
        public UsersFormModel UsersFormModelForReviewer { get; set; }
        public TopicsControllerUsersTest()
        {
            _tester = new ControllerTester<TopicsController>();
            Student1 = new UserResult
            {
                Id = "test-auth:student7",
                Email = "student1@hipapp.de",
                Roles = new[] { "Student" }
            };
            Student2 = new UserResult
            {
                Id = "test-auth:student8",
                Email = "student2@hipapp.de",
                Roles = new[] { "Student" }
            };
            Supervisor1 = new UserResult
            {
                Id = "test-auth:supervisor9",
                Email = "supervisor1@hipapp.de",
                Roles = new[] { "Supervisor" }
            };
            Supervisor2 = new UserResult
            {
                Id = "test-auth:supervisor10",
                Email = "supervisor2@hipapp.de",
                Roles = new[] { "Supervisor" }
            };
            Reviewer1 = new UserResult
            {
                Id = "test-auth:reviewer11",
                Email = "reviewer1@hipapp.de",
                Roles = new[] { "Reviewer" }
            };
            Reviewer2 = new UserResult
            {
                Id = "test-auth:reviewer12",
                Email = "reviewer2@hipapp.de",
                Roles = new[] { "Reviewer" }
            };
            UsersFormModelForStudent = new UsersFormModel
            {
                Users = new[] { Student1.Email, Student2.Email }
            };
            UsersFormModelForSupervisor = new UsersFormModel
            {
                Users = new[] { Supervisor1.Email, Supervisor2.Email }
            };
            UsersFormModelForReviewer = new UsersFormModel
            {
                Users = new[] { Reviewer1.Email, Reviewer2.Email }
            };
        }

        
        #region Get Users test
        
        /// <summary>
        /// Returns ok if topic students are retrieved
        /// </summary>
        [Fact]
        public void GetTopicStudentsTest()
        {
            _tester.TestControllerWithMockData()
                .Calling(c => c.GetTopicStudents(_tester.TopicOne.Id))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<IEnumerable<string>>()
                .Passing(actual => actual.Any(u => u == _tester.Student.Id));
        }
        
        /// <summary>
        /// Returns ok if topic supervisors are retrieved
        /// </summary>
        [Fact]
        public void GetTopicSupervisorsTest()
        {
            _tester.TestControllerWithMockData()
                .Calling(c => c.GetTopicSupervisors(_tester.TopicOne.Id))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<IEnumerable<string>>()
                .Passing(actual => actual.Any(u => u == _tester.Supervisor.Id));
        }
        
        /// <summary>
        /// Returns ok if topic supervisors are retrieved
        /// </summary>
        [Fact]
        public void GetTopicReviewersTest()
        {
            var reviewer = new UserResult
            {
                Id = "test-auth:reviewer",
                Email = "reviewer@hipapp.de",
                Roles = new[] { "Reviewer" }
            };

            var reviewerUser = new TopicUser
            {
                TopicId = _tester.TopicOne.Id,
                UserId = reviewer.Id,
                Role = reviewer.Roles.First()
            };
            
            _tester.TestControllerWithMockData()
                .WithDbContext(dbContext => dbContext
                    .WithSet<TopicUser>(db => db.Add(reviewerUser)))
                .Calling(c => c.GetTopicReviewers(_tester.TopicOne.Id))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<IEnumerable<string>>()
                .Passing(actual => actual.Any(u => u == reviewer.Id));
        }
        
        #endregion
        
        #region PUT users
        
        /// <summary>
        /// Asserts if topic students are updated
        /// </summary>
        [Fact]
        public void PutTopicStudentsTest()
        {
            _tester.TestControllerWithMockData()
                .WithDbContext(dbContext => { })
                .Calling(c => c.PutTopicStudentsAsync(_tester.TopicTwo.Id, UsersFormModelForStudent))
                .ShouldHave()
                .DbContext(db => db.WithSet<Topic>(topic =>
                    topic.Single(actual =>
                        actual.Title == _tester.TopicTwo.Title &&
                        actual.TopicUsers.Count == 2)));
            //As we have problems with DI, it does not check for any return
        }
        
        /// <summary>
        /// Returns 403 if s student is trying to update topic users
        /// </summary>
        [Fact]
        public void PutTopicStudentsTest403()
        {
            _tester.TestControllerWithMockData(_tester.Student.Id)
                .Calling(c => c.PutTopicStudentsAsync(_tester.TopicTwo.Id, UsersFormModelForStudent))
                .ShouldReturn()
                .StatusCode(403);
        }
        
        /// <summary>
        /// Returns 400 if the model is incorrect
        /// </summary>
        [Fact]
        public void PutTopicStudentsTest400()
        {
            var usersFormModel = new UsersFormModel();
            _tester.TestControllerWithMockData()
                .Calling(c => c.PutTopicStudentsAsync(_tester.TopicTwo.Id, usersFormModel))
                .ShouldReturn()
                .BadRequest();
        }
        
        /// <summary>
        /// Asserts if topic supervisors are updated
        /// </summary>
        [Fact]
        public void PutTopicSupervisorsTest()
        {
            _tester.TestControllerWithMockData()
                .WithDbContext(dbContext => { })
                .Calling(c => c.PutTopicSupervisorsAsync(_tester.TopicTwo.Id, UsersFormModelForSupervisor))
                .ShouldHave()
                .DbContext(db => db.WithSet<Topic>(topic =>
                    topic.Single(actual =>
                        actual.Title == _tester.TopicTwo.Title &&
                        actual.TopicUsers.Count == 2)));
            //As we have problems with DI, it does not check for any return
        }
        
        /// <summary>
        /// Returns 403 if s student is trying to update topic users
        /// </summary>
        [Fact]
        public void PutTopicSupervisorsTest403()
        {
            _tester.TestControllerWithMockData(_tester.Student.Id)
                .Calling(c => c.PutTopicSupervisorsAsync(_tester.TopicTwo.Id, UsersFormModelForSupervisor))
                .ShouldReturn()
                .StatusCode(403);
        }
        
        /// <summary>
        /// Returns 400 if the model is incorrect
        /// </summary>
        [Fact]
        public void PutTopicSupervisorsTest400()
        {
            var usersFormModel = new UsersFormModel();
            _tester.TestControllerWithMockData()
                .Calling(c => c.PutTopicSupervisorsAsync(_tester.TopicTwo.Id, usersFormModel))
                .ShouldReturn()
                .BadRequest();
        }
        
        /// <summary>
        /// Asserts if topic supervisors are updated
        /// </summary>
        [Fact]
        public void PutTopicReviewersTest()
        {
            _tester.TestControllerWithMockData()
                .WithDbContext(dbContext => { })
                .Calling(c => c.PutTopicReviewersAsync(_tester.TopicTwo.Id, UsersFormModelForReviewer))
                .ShouldHave()
                .DbContext(db => db.WithSet<Topic>(topic =>
                    topic.Single(actual =>
                        actual.Title == _tester.TopicTwo.Title &&
                        actual.TopicUsers.Count == 2)));
            //As we have problems with DI, it does not check for any return
        }
        
        /// <summary>
        /// Returns 403 if s student is trying to update topic users
        /// </summary>
        [Fact]
        public void PutTopicReviewersTest403()
        {
            _tester.TestControllerWithMockData(_tester.Student.Id)
                .Calling(c => c.PutTopicReviewersAsync(_tester.TopicTwo.Id, UsersFormModelForReviewer))
                .ShouldReturn()
                .StatusCode(403);
        }
        
        /// <summary>
        /// Returns 400 if the model is incorrect
        /// </summary>
        [Fact]
        public void PutTopicReviewersTest400()
        {
            var usersFormModel = new UsersFormModel();
            _tester.TestControllerWithMockData()
                .Calling(c => c.PutTopicReviewersAsync(_tester.TopicTwo.Id, usersFormModel))
                .ShouldReturn()
                .BadRequest();
        }
        
        #endregion
    }
}