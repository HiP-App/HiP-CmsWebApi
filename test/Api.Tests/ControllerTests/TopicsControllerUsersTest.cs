using MyTested.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.CmsApi.Controllers;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.User;
using System.Collections.Generic;
using System.Linq;
using Xunit;
namespace PaderbornUniversity.SILab.Hip.CmsApi.Tests.ControllerTests
{
    public class TopicsControllerUsersTest
    {
        private ControllerTester<TopicsController> _tester;
        public User Student1 { get; set; }
        public User Student2 { get; set; }
        public User Supervisor1 { get; set; }
        public User Supervisor2 { get; set; }
        public User Reviewer1 { get; set; }
        public User Reviewer2 { get; set; }
        public UsersFormModel UsersFormModelForStudent { get; set; }
        public UsersFormModel UsersFormModelForSupervisor { get; set; }
        public UsersFormModel UsersFormModelForReviewer { get; set; }
        public TopicsControllerUsersTest()
        {
            _tester = new ControllerTester<TopicsController>();
            Student1 = new User
            {
                Id = 5,
                Email = "student1@hipapp.de",
                Role = "Student"
            };
            Student2 = new User
            {
                Id = 6,
                Email = "student2@hipapp.de",
                Role = "Student"
            };
            Supervisor1 = new User
            {
                Id = 5,
                Email = "supervisor1@hipapp.de",
                Role = "Supervisor"
            };
            Supervisor2 = new User
            {
                Id = 6,
                Email = "supervisor2@hipapp.de",
                Role = "Supervisor"
            };
            Reviewer1 = new User
            {
                Id = 5,
                Email = "reviewer1@hipapp.de",
                Role = "Reviewer"
            };
            Reviewer2 = new User
            {
                Id = 6,
                Email = "reviewer2@hipapp.de",
                Role = "Reviewer"
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
            var model = new UserResult(_tester.Student) { Identity = _tester.Student.Email };
            _tester.TestControllerWithMockData()
                .Calling(c => c.GetTopicStudents(_tester.TopicOne.Id))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<IEnumerable<UserResult>>()
                .Equals(model);
        }
        /// <summary>
        /// Returns ok if topic supervisors are retrieved
        /// </summary>
        [Fact]
        public void GetTopicSupervisorsTest()
        {
            var model = new UserResult(_tester.Supervisor) { Identity = _tester.Supervisor.Email };
            _tester.TestControllerWithMockData()
                .Calling(c => c.GetTopicSupervisors(_tester.TopicOne.Id))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<IEnumerable<UserResult>>()
                .Equals(model);
        }
        /// <summary>
        /// Returns ok if topic supervisors are retrieved
        /// </summary>
        [Fact]
        public void GetTopicReviewersTest()
        {
            var reviewer = new User
            {
                Id = 4,
                Email = "reviewer@hipapp.de",
                Role = "Reviewer"
            };
            var reviewerUser = new TopicUser
            {
                TopicId = _tester.TopicOne.Id,
                UserId = reviewer.Id,
                Role = reviewer.Role
            };
            var model = new UserResult(_tester.Supervisor) { Identity = reviewer.Email };
            _tester.TestControllerWithMockData()
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(reviewer))
                    .WithSet<TopicUser>(db => db.Add(reviewerUser)))
                .Calling(c => c.GetTopicReviewers(_tester.TopicOne.Id))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<IEnumerable<UserResult>>()
                .Equals(model);
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
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.AddRange(Student1, Student2)))
                .Calling(c => c.PutTopicStudents(_tester.TopicTwo.Id, UsersFormModelForStudent))
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
            _tester.TestControllerWithMockData(_tester.Student.Email)
                .Calling(c => c.PutTopicStudents(_tester.TopicTwo.Id, UsersFormModelForStudent))
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
                .Calling(c => c.PutTopicStudents(_tester.TopicTwo.Id, usersFormModel))
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
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.AddRange(Supervisor1, Supervisor2)))
                .Calling(c => c.PutTopicSupervisors(_tester.TopicTwo.Id, UsersFormModelForSupervisor))
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
            _tester.TestControllerWithMockData(_tester.Student.Email)
                .Calling(c => c.PutTopicSupervisors(_tester.TopicTwo.Id, UsersFormModelForSupervisor))
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
                .Calling(c => c.PutTopicSupervisors(_tester.TopicTwo.Id, usersFormModel))
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
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.AddRange(Reviewer1, Reviewer2)))
                .Calling(c => c.PutTopicReviewers(_tester.TopicTwo.Id, UsersFormModelForReviewer))
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
            _tester.TestControllerWithMockData(_tester.Student.Email)
                .Calling(c => c.PutTopicReviewers(_tester.TopicTwo.Id, UsersFormModelForReviewer))
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
                .Calling(c => c.PutTopicReviewers(_tester.TopicTwo.Id, usersFormModel))
                .ShouldReturn()
                .BadRequest();
        }
        #endregion
    }
}