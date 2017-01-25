using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Controllers;
using Api.Managers;
using Api.Models;
using Api.Models.Entity;
using MyTested.AspNetCore.Mvc;
using NUnit.Framework;

namespace Api.Tests.ControllerTests
{
    [TestFixture]
    public class AnnotationTagRelationsControllerTest
    {
        private User _admin;
        private User _student;
        private AnnotationTag _tag1;
        private AnnotationTag _tag2;
        private AnnotationTag _tag3;
        private TagRelation _relation12;

        [SetUp]
        public void BeforeTest()
        {
            // create some User, Tag and TagRelation objects for mocking the database
            _admin = new User
            {
                Id = 1,
                Email = "admin@hipapp.de",
                Role = "Administrator"
            };
            _student = new User
            {
                Id = 2,
                Email = "student@hipapp.de",
                Role = "Student"
            };
            _tag1 = new AnnotationTag() { Id = 1 };
            _tag2 = new AnnotationTag() { Id = 2 };
            _tag3 = new AnnotationTag() { Id = 3 };
            _relation12 = new TagRelation(); // TODO: Pass tag1 and tag2 as parameters as soon as the TagRelation object is fixed
        }

        #region GetRelations

        /// <summary>
        /// Should return code 200 and a list of all tag relations if called properly
        /// </summary>
        [Test]
        public void GetRelationsTest()
        {
            var expected = new List<TagRelation>() { _relation12 };
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext.WithSet<TagRelation>(db => db.Add(_relation12)))
                .Calling(c => c.GetRelations(0))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<List<TagRelation>>()
                .Passing(actual => expected.SequenceEqual(actual));
        }

        /// <summary>
        /// Should return 400 for negative maxDepth values
        /// </summary>
        [Test]
        public void GetRelationsTest400()
        {
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .Calling(c => c.GetRelations(-1))
                .ShouldReturn()
                .BadRequest();
        }

        #endregion

        #region GetRelationsForId

        /// <summary>
        /// Should return code 200 and an empty list of tag relations if called for an existing tag that has no relations
        /// </summary>
        [Test]
        public void GetRelationsForIdWithNoExistingRelationsTest()
        {
            // ReSharper disable once CollectionNeverUpdated.Local
            var expected = new List<TagRelation>();
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(_admin))
                    .WithSet<AnnotationTag>(db => db.AddRange(_tag1, _tag2))
                )
                .Calling(c => c.GetRelationsForId(_tag1.Id))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<List<TagRelation>>()
                .Passing(actual => expected.SequenceEqual(actual));
        }

        /// <summary>
        /// Should return code 200 and a list of all tag relations if called properly for an existing tag with relations
        /// </summary>
        [Test]
        public void GetRelationsForIdWithOneExistingRelationTest()
        {
            var expected = new List<TagRelation>() { _relation12 };
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(_admin))
                    .WithSet<AnnotationTag>(db => db.AddRange(_tag1, _tag2))
                    .WithSet<TagRelation>(db => db.Add(_relation12))
                )
                .Calling(c => c.GetRelationsForId(_tag1.Id))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<List<TagRelation>>()
                .Passing(actual => expected.SequenceEqual(actual));
        }

        /// <summary>
        /// Relations are uni-directional i.e. tag2 (the tag with the INCOMING relation, but no outgoing relations) should have no relations
        /// </summary>
        [Test]
        public void GetRelationsForIdUniDirectionalTest()
        {
            // ReSharper disable once CollectionNeverUpdated.Local
            var expected = new List<TagRelation>();
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(_admin))
                    .WithSet<AnnotationTag>(db => db.AddRange(_tag1, _tag2))
                    .WithSet<TagRelation>(db => db.Add(_relation12))
                )
                .Calling(c => c.GetRelationsForId(_tag2.Id))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<List<TagRelation>>()
                .Passing(actual => expected.SequenceEqual(actual));
        }

        /// <summary>
        /// Should return 400 for negative maxDepth values
        /// </summary>
        [Test]
        public void GetRelationsForIdTest400()
        {
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .Calling(c => c.GetRelationsForId(1))
                .ShouldReturn()
                .BadRequest();
        }

        #endregion

        #region PostTagRelation

        /// <summary>
        /// Should return code 200 if called with ids of two existing tags that do not have a relation yet
        /// </summary>
        [Test]
        public void PostTagRelationTest()
        {
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(_admin))
                    .WithSet<AnnotationTag>(db => db.AddRange(_tag1, _tag2))
                )
                .Calling(c => c.PostTagRelation(_tag1.Id, _tag2.Id, "myrelation"))
                .ShouldReturn()
                .Ok();
        }

        /// <summary>
        /// Should return 400 for tags that do not exist
        /// </summary>
        [Test]
        public void PostTagRelationTest400()
        {
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext.WithSet<User>(db => db.Add(_admin)))
                // --> tags 1 and 2 were NOT added to the database
                .Calling(c => c.PostTagRelation(1, 2, "myrelation"))
                .ShouldReturn()
                .BadRequest();
        }

        /// <summary>
        /// Should return 403 for users with the student role
        /// </summary>
        [Test]
        public void PostTagRelationTest403()
        {
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "2"))
                .WithDbContext(dbContext => dbContext.WithSet<User>(db => db.Add(_student)))
                .Calling(c => c.PostTagRelation(1, 2, "myrelation"))
                .ShouldReturn()
                .Forbid();
        }

        #endregion

        #region PutTagRelation

        /// <summary>
        /// Should return code 200 if called with ids of two existing tags that have a relation
        /// </summary>
        [Test]
        public void PutTagRelationTest()
        {
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(_admin))
                    .WithSet<AnnotationTag>(db => db.AddRange(_tag1, _tag2))
                    .WithSet<TagRelation>(db => db.Add(_relation12))
                )
                .Calling(c => c.PutTagRelation(_relation12.FirstTagId, _relation12.SecondTagId, "mychangedrelation"))
                .ShouldReturn()
                .Ok();
        }

        /// <summary>
        /// Should return 400 for relations that do not exist
        /// </summary>
        [Test]
        public void PutTagRelationTest400()
        {
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(_admin))
                    .WithSet<AnnotationTag>(db => db.AddRange(_tag1, _tag2))
                )
                .Calling(c => c.PutTagRelation(_relation12.FirstTagId, _relation12.SecondTagId, "mychangedrelation"))
                .ShouldReturn()
                .BadRequest();
        }
        
        /// <summary>
        /// Should return 403 for users with the student role
        /// </summary>
        [Test]
        public void PutTagRelationTest403()
        {
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "2"))
                .WithDbContext(dbContext => dbContext.WithSet<User>(db => db.Add(_student)))
                .Calling(c => c.PutTagRelation(_relation12.FirstTagId, _relation12.SecondTagId, "mychangedrelation"))
                .ShouldReturn()
                .Forbid();
        }

        #endregion

        #region DeleteTagRelation


        /// <summary>
        /// Should return code 200 if called for an existing TagRelation
        /// </summary>
        [Test]
        public void DeleteTagRelationTest()
        {
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(_admin))
                    .WithSet<AnnotationTag>(db => db.AddRange(_tag1, _tag2))
                    .WithSet<TagRelation>(db => db.Add(_relation12))
                )
                .Calling(c => c.DeleteTagRelation(_relation12.FirstTagId, _relation12.SecondTagId))
                .ShouldReturn()
                .Ok();
        }

        /// <summary>
        /// Should return 400 for relations that do not exist
        /// </summary>
        [Test]
        public void DeleteTagRelationTest400()
        {
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(_admin))
                    .WithSet<AnnotationTag>(db => db.AddRange(_tag1, _tag2))
                )
                // --> no TagRelation objects were added to the database
                .Calling(c => c.DeleteTagRelation(1, 2))
                .ShouldReturn()
                .BadRequest();
        }

        /// <summary>
        /// Should return 403 for users with the student role
        /// </summary>
        [Test]
        public void DeleteTagRelationTest403()
        {
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "2"))
                .WithDbContext(dbContext => dbContext.WithSet<User>(db => db.Add(_student)))
                .Calling(c => c.DeleteTagRelation(1, 2))
                .ShouldReturn()
                .Forbid();
        }

        #endregion
    }
}
