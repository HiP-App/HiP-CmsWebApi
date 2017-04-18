using MyTested.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.CmsApi.Controllers;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity;
using System;
using Xunit;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Tests.ControllerTests
{
    public class PermissionControllerTest
    {
        private ControllerTester<PermissionsController> _tester;        
        public Topic Topic { get; set; }
        public TopicUser SupervisorUser { get; set; }
        public TopicUser StudentUser { get; set; }        

        public PermissionControllerTest()
        {
            _tester = new ControllerTester<PermissionsController>();            
            Topic = new Topic
            {
                Id = 1,
                Title = "Paderborner Dom",
                Status = "InReview",
                Deadline = new DateTime(2017, 4, 18),
                CreatedById = _tester.Supervisor.Id
            };
            SupervisorUser = new TopicUser
            {
                TopicId = Topic.Id,
                UserId = _tester.Supervisor.Id,
                Role = _tester.Supervisor.Role
            };
            StudentUser = new TopicUser
            {
                TopicId = Topic.Id,
                UserId = _tester.Student.Id,
                Role = _tester.Student.Role
            };            
        }

        #region Annotations test

        /// <summary>
        /// Should return ok if admin tries to create tags
        /// </summary>
        [Fact]
        public void IsAllowedToCreateTagsTestOk()
        {
            _tester.TestController()
                .Calling(c => c.IsAllowedToCreateTags())
                .ShouldReturn()
                .Ok();
        }

        /// <summary>
        /// Should return Unauthorized if student tries to create tags
        /// </summary>
        [Fact]
        public void IsAllowedToCreateTagsTestUnauthorized()
        {
            _tester.TestController(_tester.Student.Email)
                .Calling(c => c.IsAllowedToCreateTags())
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
                .Calling(c => c.IsAllowedToEditTags())
                .ShouldReturn()
                .Ok();
        }

        /// <summary>
        /// Should return Unauthorized if student tries to edit tags
        /// </summary>
        [Fact]
        public void IsAllowedToEditTagsTestUnauthorized()
        {
            _tester.TestController(_tester.Student.Email)
                .Calling(c => c.IsAllowedToEditTags())
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
                .Calling(c => c.IsAllowedToCreate())
                .ShouldReturn()
                .Ok();
        }

        /// <summary>
        /// Should return status code 403 if student tries to create topics
        /// </summary>
        [Fact]
        public void IsAllowedToCreateTestForbidden()
        {
            _tester.TestController(_tester.Student.Email)
                .Calling(c => c.IsAllowedToCreate())
                .ShouldReturn()
                .StatusCode(403);
        }

        /// <summary>
        /// Should return ok if admin tries to edit topics
        /// </summary>
        [Fact]
        public void IsAllowedToEditTestOk()
        {
            _tester.TestController()
                .Calling(c => c.IsAllowedToCreate())
                .ShouldReturn()
                .Ok();
        }

        /// <summary>
        /// Should return status code 403 if student tries to edit topics
        /// </summary>
        [Fact]
        public void IsAllowedToEditTestForbidden()
        {
            _tester.TestController(_tester.Student.Email)
                .Calling(c => c.IsAllowedToCreate())
                .ShouldReturn()
                .StatusCode(403);
        }

        /// <summary>
        /// Should return ok if admin tries to associate with the topic
        /// </summary>
        [Fact]
        public void IsAssociatedToTestOk()
        {
            _tester.TestController()
                .WithDbContext(dbContext => dbContext.WithSet<Topic>(db => db.Add(Topic)))
                .Calling(c => c.IsAssociatedTo(Topic.Id))
                .ShouldReturn()
                .Ok();
        }

        /// <summary>
        /// Should return ok if supervisor has created a topic and he tries to associate with the topic
        /// </summary>
        [Fact]
        public void IsAssociatedToTestForSupervisor()
        {
            _tester.TestController(_tester.Supervisor.Email)
                .WithDbContext(dbContext => dbContext
                    .WithSet<Topic>(db => db.Add(Topic))
                    .WithSet<TopicUser>(db => db.AddRange(SupervisorUser, StudentUser)))
                .Calling(c => c.IsAssociatedTo(Topic.Id))
                .ShouldReturn()
                .Ok();
        }

        /// <summary>
        /// Should return ok if student tries to associate with the topic
        /// </summary>
        [Fact]
        public void IsAssociatedToTestForStudent()
        {
            _tester.TestController(_tester.Student.Email)
                .WithDbContext(dbContext => dbContext
                    .WithSet<Topic>(db => db.Add(Topic))
                    .WithSet<TopicUser>(db => db.AddRange(SupervisorUser, StudentUser)))
                .Calling(c => c.IsAssociatedTo(Topic.Id))
                .ShouldReturn()
                .Ok();
        }

        /// <summary>
        /// Should return code 403 if a new student tries to associate with the topic
        /// </summary>
        [Fact]
        public void IsAssociatedToTestForbidden()
        {
            var newStudent = new User
            {
                Id = 4,
                Email = "student1@hipapp.de",
                Role = "Student"
            };

            _tester.TestController(newStudent.Email)
                .WithDbContext(dbContext => dbContext
                    .WithSet<Topic>(db => db.Add(Topic))
                    .WithSet<TopicUser>(db => db.AddRange(SupervisorUser, StudentUser))
                    .WithSet<User>(db => db.Add(newStudent)))
                .Calling(c => c.IsAssociatedTo(Topic.Id))
                .ShouldReturn()
                .StatusCode(403);
        }

        /// <summary>
        /// Should return ok if reviewer tries to associate with the topic
        /// </summary>
        [Fact]
        public void IsAllowedToReviewTest()
        {
            var reviewer = new User
            {
                Id = 4,
                Email = "reviewer@hipapp.de",
                Role = "Reviewer"
            };

            var ReviewerUser = new TopicUser
            {
                TopicId = Topic.Id,
                UserId = reviewer.Id,
                Role = reviewer.Role
            };

            _tester.TestController(reviewer.Email)
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(reviewer))
                    .WithSet<Topic>(db => db.Add(Topic))
                    .WithSet<TopicUser>(db => db.AddRange(SupervisorUser, StudentUser,ReviewerUser)))
                .Calling(c => c.IsAllowedToReview(Topic.Id))
                .ShouldReturn()
                .Ok();
        }

        /// <summary>
        /// Should return code 403 if admin tries to associate with the topic
        /// </summary>
        [Fact]
        public void IsAllowedToReviewTest403ForAdmin()
        {
            _tester.TestController()
                .WithDbContext(dbContext => dbContext                    
                    .WithSet<Topic>(db => db.Add(Topic))
                    .WithSet<TopicUser>(db => db.AddRange(SupervisorUser, StudentUser)))
                .Calling(c => c.IsAllowedToReview(Topic.Id))
                .ShouldReturn()
                .StatusCode(403);
        }

        /// <summary>
        /// Should return code 403 if student tries to associate with the topic
        /// </summary>
        [Fact]
        public void IsAllowedToReviewTest403ForStudent()
        {
            _tester.TestController(_tester.Student.Email)
                .WithDbContext(dbContext => dbContext
                    .WithSet<Topic>(db => db.Add(Topic))
                    .WithSet<TopicUser>(db => db.AddRange(SupervisorUser, StudentUser)))
                .Calling(c => c.IsAllowedToReview(Topic.Id))
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
            _tester.TestController(_tester.Student.Email)
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
            _tester.TestController(_tester.Supervisor.Email)
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
            _tester.TestController(_tester.Student.Email)
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
            _tester.TestController(_tester.Supervisor.Email)
                .Calling(c => c.IsAllowedToInvite())
                .ShouldReturn()
                .Ok();
        }
        #endregion

    }
}