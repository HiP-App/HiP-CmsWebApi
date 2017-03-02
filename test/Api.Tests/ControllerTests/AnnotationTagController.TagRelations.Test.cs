using System.Collections.Generic;
using System.Linq;
using Api.Controllers;
using Api.Models.Entity;
using Api.Models.AnnotationTag;
using Api.Models.Entity.Annotation;
using MyTested.AspNetCore.Mvc;
using NUnit.Framework;
// TODO fix ReSharper
// ReSharper disable AccessToModifiedClosure
// ReSharper disable UnusedVariable
// ReSharper disable CollectionNeverUpdated.Local

namespace Api.Tests.ControllerTests
{
    [TestFixture]
    public class AnnotationTagRelationsControllerTest
    {
        private User _admin;
        private User _student;
        private User _supervisor;
        private AnnotationTag _tag1;
        private AnnotationTag _tag2;
        private AnnotationTag _tag3;
        private AnnotationTag _tag4;
        private AnnotationTagRelation _relation12;
        private AnnotationTagRelation _relation34;
        private Layer _layer2;
        private Layer _layer1;
        private LayerRelationRule _layerRelationRule;

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
            _supervisor = new User
            {
                Id = 3,
                Email = "supervisor@hipapp.de",
                Role = "Supervisor"
            };
            /*
             * Layer1   Layer2
             * |-tag1   |-tag2
             * |-tag3   |-tag4
             * 
             * Layer Relation Rules:
             * Layer1 -> Layer2
             * 
             * Annotation Tag Relations:
             * tag1 -> tag2
             * tag3 -> tag2
             * tag3 -> tag4
             */
            _layer1 = new Layer() { Id = 1, Name = "Time" };
            _layer2 = new Layer() { Id = 2, Name = "Perspective" };
            _tag1 = new AnnotationTag() { Id = 1, Layer = _layer1.Name };
            _tag2 = new AnnotationTag() { Id = 2, Layer = _layer2.Name };
            _tag3 = new AnnotationTag() { Id = 3, Layer = _layer1.Name };
            _tag4 = new AnnotationTag() { Id = 4, Layer = _layer2.Name };
            _tag1.ChildTags = new List<AnnotationTag>() { _tag3 };
            _tag2.ChildTags = new List<AnnotationTag>() { _tag4 };
            _relation12 = new AnnotationTagRelation(_tag1, _tag2);
            _relation34 = new AnnotationTagRelation(_tag3, _tag4);
            _layerRelationRule = new LayerRelationRule()
            {
                SourceLayer = _layer1,
                SourceLayerId = _layer1.Id,
                TargetLayer = _layer2,
                TargetLayerId = _layer2.Id,
                Color = "test-color",
                ArrowStyle = "test-style"
            };
        }

        #region GetLayerRelationRules

        /// <summary>
        /// Should return code 200 and a list of all layer relation rules if called properly.
        /// Layer Relations can have a title, description, color and arrow-style.
        /// </summary>
        [Test]
        public void GetLayerRelationRulesTest()
        {
            var myRelation = new LayerRelationRule()
            {
                SourceLayer = _layer1,
                SourceLayerId = _layer1.Id,
                TargetLayer = _layer2,
                TargetLayerId = _layer2.Id,
                Color = "my-color",
                ArrowStyle = "my-style",
                Title = "my-title",
                Description= "my-description"
            };
            var expected = new List<LayerRelationRule>() { myRelation };
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext
                    .WithSet<Layer>(db => db.AddRange(_layer1, _layer2))
                    .WithSet<LayerRelationRule>(db => db.Add(myRelation))
                )
                .Calling(c => c.GetAllLayerRelationRules())
                .ShouldReturn()
                .Ok()
                .WithModelOfType<List<LayerRelationRule>>()
                .Passing(actual => expected.SequenceEqual(actual));
        }

        #endregion

        #region PostLayerRelationRule

        /// <summary>
        /// Should return code 200 and create the layer relation rule if called properly
        /// </summary>
        [Test]
        public void PostLayerRelationRuleTest()
        {
            var expected = _layerRelationRule;
            var model = new LayerRelationRuleFormModel()
            {
                SourceLayerId = expected.SourceLayerId,
                TargetLayerId = expected.TargetLayerId,
                Color = expected.Color,
                ArrowStyle = expected.ArrowStyle
            };
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", _supervisor.Id.ToString()))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(_supervisor))
                    .WithSet<Layer>(db => db.AddRange(_layerRelationRule.SourceLayer, _layerRelationRule.TargetLayer))
                )
                .Calling(c => c.PostLayerRelationRule(model))
                .ShouldHave()
                .DbContext(db => db.WithSet<LayerRelationRule>(relations =>
                    relations.Any(actual =>
                        expected.SourceLayerId == actual.SourceLayerId &&
                        expected.TargetLayerId == actual.TargetLayerId &&
                        expected.Color == actual.Color &&
                        expected.ArrowStyle == actual.ArrowStyle
                    )
                ))
                .AndAlso()
                .ShouldReturn()
                .Ok();

        }

        /// <summary>
        /// Should return code 403 if a student tries to create layer relation rules
        /// </summary>
        [Test]
        public void PostLayerRelationRuleTest403()
        {
            var expected = _layerRelationRule;
            var model = new LayerRelationRuleFormModel()
            {
                SourceLayerId = expected.SourceLayerId,
                TargetLayerId = expected.TargetLayerId,
                Color = expected.Color,
                ArrowStyle = expected.ArrowStyle
            };
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", _student.Id.ToString()))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(_student))
                    .WithSet<Layer>(db => db.AddRange(expected.SourceLayer, expected.TargetLayer))
                )
                .Calling(c => c.PostLayerRelationRule(model))
                .ShouldHave()
                .DbContext(db => db.WithSet<LayerRelationRule>(relations =>
                    !relations.Any(actual => actual.Equals(expected))
                ))
                .AndAlso()
                .ShouldReturn()
                .Forbid();
        }

        #endregion

        #region GetRelations

        /// <summary>
        /// Should return code 200 and a list of all tag relations if called properly
        /// </summary>
      // TODO  [Test]
        public void GetRelationsTest()
        {
            var expected = new List<AnnotationTagRelation>() { _relation12 };
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext.WithSet<AnnotationTagRelation>(db => db.Add(_relation12)))
                .Calling(c => c.GetRelations(0))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<List<AnnotationTagRelation>>()
                .Passing(actual => expected.SequenceEqual(actual));
        }

        /// <summary>
        /// Should return 400 for negative maxDepth values
        /// </summary>
       // TODO [Test]
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
        // TODO [Test]
        public void GetRelationsForIdWithNoExistingRelationsTest()
        {
            // ReSharper disable once CollectionNeverUpdated.Local
            var expected = new List<AnnotationTagRelation>();
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
                .WithModelOfType<List<AnnotationTagRelation>>()
                .Passing(actual => expected.SequenceEqual(actual));
        }

        /// <summary>
        /// Should return code 200 and a list of all tag relations if called properly for an existing tag with relations
        /// </summary>
        // TODO [Test]
        public void GetRelationsForIdWithOneExistingRelationTest()
        {
            var expected = new List<AnnotationTagRelation>() { _relation12 };
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(_admin))
                    .WithSet<AnnotationTag>(db => db.AddRange(_tag1, _tag2))
                    .WithSet<AnnotationTagRelation>(db => db.Add(_relation12))
                )
                .Calling(c => c.GetRelationsForId(_tag1.Id))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<List<AnnotationTagRelation>>()
                .Passing(actual => expected.SequenceEqual(actual));
        }

        /// <summary>
        /// Relations are uni-directional i.e. tag2 (the tag with the INCOMING relation, but no outgoing relations) should have no relations
        /// </summary>
        // TODO [Test]
        public void GetRelationsForIdUniDirectionalTest()
        {
            // ReSharper disable once CollectionNeverUpdated.Local
            var expected = new List<AnnotationTagRelation>();
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(_admin))
                    .WithSet<AnnotationTag>(db => db.AddRange(_tag1, _tag2))
                    .WithSet<AnnotationTagRelation>(db => db.Add(_relation12))
                )
                .Calling(c => c.GetRelationsForId(_tag2.Id))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<List<AnnotationTagRelation>>()
                .Passing(actual => expected.SequenceEqual(actual));
        }

        /// <summary>
        /// Should return 400 for negative maxDepth values
        /// </summary>
        // TODO [Test]
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

        #region GetAllowedRelationRulesForTag

        /// <summary>
        /// Should return code 200 and a list of all tags that relation rules are allowed to if called properly.
        /// Duplicate relations are also allowed --> tag2 is also expected to be in the returned list
        /// </summary>
        [Test]
        public void GetAllowedRelationRulesForTagTest()
        {
            var expected = new List<AnnotationTag>() { _tag2, _tag4 };
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(_admin))
                    .WithSet<AnnotationTag>(db => db.AddRange(_tag1, _tag2, _tag3, _tag4))
                    .WithSet<Layer>(db => db.AddRange(_layer1, _layer2))
                    .WithSet<LayerRelationRule>(db => db.Add(_layerRelationRule))
                )
                .Calling(c => c.GetAllowedRelationRulesForTag(_tag1.Id))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<List<AnnotationTag>>()
                .Passing(actual => expected.SequenceEqual(actual));
        }        

        /// <summary>
        /// Should return code 200 and an empty list tags if there are no relations possible because
        /// the top-level tags do not have a layer relation rule defined
        /// </summary>
        [Test]
        public void GetAllowedRelationRulesForTagTest_NoToplevelRelation()
        {
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(_admin))
                    .WithSet<AnnotationTag>(db => db.AddRange(_tag1, _tag2, _tag3, _tag4))
                    .WithSet<Layer>(db => db.AddRange(_layer1, _layer2))
                    .WithSet<LayerRelationRule>(db => db.Add(_layerRelationRule))
                )
                // no layer relation rules exist from layer2 to layer1 --> no relations from tag2 to tag1 / tag3 allowed
                .Calling(c => c.GetAllowedRelationRulesForTag(_tag2.Id))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<List<AnnotationTag>>()
                .Passing(actual => !actual.Any());
        }

        /// <summary>
        /// Should return 400 for tags that do not exist
        /// </summary>
       [Test]
        public void GetAllowedRelationRulesForTagTest400()
        {
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .Calling(c => c.GetAllowedRelationRulesForTag(_tag1.Id))
                .ShouldReturn()
                .BadRequest();
        }

        #endregion

        #region GetAllowedRelationRulesForTag

        /// <summary>
        /// Should return code 200 and a list of all tag relations that are available for the given tag instance
        /// </summary>
      // TODO  [Test]
        public void GetAvailableRelationsForIdTest()
        {
            var tag5 = new AnnotationTag() { Id = 5 };
            var relation35 = new AnnotationTagRelation(_tag3, tag5);
            var expected = new List<AnnotationTagRelation>() { _relation34, relation35 };
            var instance3 = new AnnotationTagInstance(_tag3);
            var instances = new List<AnnotationTagInstance>()
            {
                new AnnotationTagInstance(_tag1),
                new AnnotationTagInstance(_tag2),
                instance3,
                new AnnotationTagInstance(_tag4),
                new AnnotationTagInstance(tag5)
            };
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(_admin))
                    .WithSet<AnnotationTag>(db => db.AddRange(_tag1, _tag2, _tag3, _tag4))
                    .WithSet<AnnotationTagRelation>(db => db.AddRange(_relation12, _relation34, relation35))
                // TODO How to model that the tag instances are part of the same document?
                )
                .Calling(c => c.GetAvailableRelationsForInstance(instance3.Id))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<List<AnnotationTagRelation>>()
                .Passing(actual => expected.SequenceEqual(actual));
        }

        /// <summary>
        /// Should return code 200 and an empty list of tag relations if there are no relations possible for the given tag instance
        /// </summary>
       // TODO [Test]
        public void GetAvailableRelationsForIdTest_NoRelations()
        {
            var expected = new List<AnnotationTagRelation>();
            var instance3 = new AnnotationTagInstance(_tag3);
            var instances = new List<AnnotationTagInstance>()
            {
                new AnnotationTagInstance(_tag1),
                new AnnotationTagInstance(_tag2),
                instance3,
                new AnnotationTagInstance(_tag4)
            };
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(_admin))
                    .WithSet<AnnotationTag>(db => db.AddRange(_tag1, _tag2, _tag3, _tag4))
                // no relations exist between the tags
                // TODO How to model that the tag instances are part of the same document?
                )
                .Calling(c => c.GetAvailableRelationsForInstance(_tag3.Id))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<List<AnnotationTagRelation>>()
                .Passing(actual => expected.SequenceEqual(actual));
        }

        /// <summary>
        /// Should return 400 for tags that do not exist
        /// </summary>
      // TODO  [Test]
        public void GetAvailableRelationsForIdTest400()
        {
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .Calling(c => c.GetAvailableRelationsForInstance(_tag3.Id))
                .ShouldReturn()
                .BadRequest();
        }

        #endregion

        #region PostTagRelation

        /// <summary>
        /// Should return code 200 if called with ids of two existing tags that do not have a relation yet
        /// </summary>
       // TODO [Test]
        public void PostTagRelationTest()
        {
            var expected = new AnnotationTagRelationFormModel()
            {
                FirstTagId = _tag1.Id,
                SecondTagId = _tag2.Id,
                Name = "relationName",
                Color = "schwarzgelb",
                ArrowStyle = "dotted"
            };
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(_admin))
                    .WithSet<AnnotationTag>(db => db.AddRange(_tag1, _tag2))
                )
                .Calling(c => c.PostTagRelation(expected))
                .ShouldHave()
                .DbContext(db => db.WithSet<AnnotationTagRelation>(relations =>
                    relations.Any(actual =>
                        actual.FirstTagId == expected.FirstTagId &&
                        actual.SecondTagId == expected.SecondTagId &&
                        actual.Name == expected.Name &&
                        actual.Color == expected.Color &&
                        actual.ArrowStyle == expected.ArrowStyle
                    )
                ))
                .AndAlso()
                .ShouldReturn()
                .Ok();
        }

        /// <summary>
        /// Should return 400 for relations that are not allowed (child tag to top-level tag)
        /// </summary>
       // TODO [Test]
        public void PostTagRelationTest_NoChildToFirstLevelRelation()
        {
            var expected = new AnnotationTagRelationFormModel()
            {
                FirstTagId = _tag3.Id,
                SecondTagId = _tag2.Id,
                Name = "child-to-toplevel-relation"
            };
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(_admin))
                    .WithSet<AnnotationTag>(db => db.AddRange(_tag1, _tag2, _tag3))
                )
                .Calling(c => c.PostTagRelation(expected))
                .ShouldHave()
                .DbContext(db => db.WithSet<AnnotationTagRelation>(relations =>
                    !relations.Any(actual => // negated --> NO relation like this exists
                        actual.FirstTagId == expected.FirstTagId &&
                        actual.SecondTagId == expected.SecondTagId &&
                        actual.Name == expected.Name
                    )
                ))
                .AndAlso()
                .ShouldReturn()
                .BadRequest();

            // other way around is also not allowed:
            expected = new AnnotationTagRelationFormModel()
            {
                FirstTagId = _tag2.Id,
                SecondTagId = _tag3.Id,
                Name = "toplevel-to-child-relation"
            };
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(_admin))
                    .WithSet<AnnotationTag>(db => db.AddRange(_tag1, _tag2, _tag3))
                )
                .Calling(c => c.PostTagRelation(expected))
                .ShouldHave()
                .DbContext(db => db.WithSet<AnnotationTagRelation>(relations =>
                    !relations.Any(actual => // negated --> NO relation like this exists
                        actual.FirstTagId == expected.FirstTagId &&
                        actual.SecondTagId == expected.SecondTagId &&
                        actual.Name == expected.Name
                    )
                ))
                .AndAlso()
                .ShouldReturn()
                .BadRequest();
        }

        /// <summary>
        /// Should return 400 for duplicate tag relations
        /// </summary>
     // TODO   [Test]
        public void PostTagRelationTest_NoDuplicateRelations()
        {
            var expected = new AnnotationTagRelationFormModel()
            {
                FirstTagId = _relation12.FirstTag.Id,
                SecondTagId = _relation12.SecondTag.Id,
                Name = "duplcate-relation"
            };
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(_admin))
                    .WithSet<AnnotationTag>(db => db.AddRange(_tag1, _tag2))
                    .WithSet<AnnotationTagRelation>(db => db.Add(_relation12))
                )
                .Calling(c => c.PostTagRelation(expected))
                .ShouldHave()
                .DbContext(db => db.WithSet<AnnotationTagRelation>(relations =>
                    relations.Count(actual =>
                        actual.FirstTagId == expected.FirstTagId &&
                        actual.SecondTagId == expected.SecondTagId &&
                        actual.Name == expected.Name
                    ) == 1 // should only contain 1 entry, not two
                ))
                .AndAlso()
                .ShouldReturn()
                .BadRequest();
        }

        /// <summary>
        /// Should return 400 for tags that do not exist
        /// </summary>
        [Test]
        public void PostTagRelationTest400()
        {
            var expected = new AnnotationTagRelationFormModel()
            {
                FirstTagId = _tag1.Id,
                SecondTagId = _tag2.Id,
                Name = "relation-with-nonexisting-tags"
            };
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext.WithSet<User>(db => db.Add(_admin)))
                // --> tags 1 and 2 were NOT added to the database
                .Calling(c => c.PostTagRelation(expected))
                .ShouldHave()
                .DbContext(db => db.WithSet<AnnotationTagRelation>(relations =>
                    !relations.Any(actual => // negated --> NO relation like this exists
                        actual.FirstTagId == expected.FirstTagId &&
                        actual.SecondTagId == expected.SecondTagId &&
                        actual.Name == expected.Name
                    )
                ))
                .AndAlso()
                .ShouldReturn()
                .BadRequest();
        }

        /// <summary>
        /// Should return 403 for users with the student role
        /// </summary>
        [Test]
        public void PostTagRelationTest403()
        {
            var expected = new AnnotationTagRelationFormModel()
            {
                FirstTagId = _tag1.Id,
                SecondTagId = _tag2.Id,
                Name = "relation-with-nonexisting-tags"
            };
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "2"))
                .WithDbContext(dbContext => dbContext.WithSet<User>(db => db.Add(_student)))
                .Calling(c => c.PostTagRelation(expected))
                .ShouldHave()
                .DbContext(db => db.WithSet<AnnotationTagRelation>(relations =>
                    !relations.Any(actual => // negated --> NO relation like this exists
                        actual.FirstTagId == expected.FirstTagId &&
                        actual.SecondTagId == expected.SecondTagId &&
                        actual.Name == expected.Name
                    )
                ))
                .AndAlso()
                .ShouldReturn()
                .Forbid();
        }

        #endregion

        #region PutTagRelation

        /// <summary>
        /// Should return code 200 if called with ids of two existing tags that have a relation
        /// </summary>
        // TODO [Test]
        public void PutTagRelationTest()
        {
            var expected = new AnnotationTagRelationFormModel()
            {
                FirstTagId = _relation12.FirstTag.Id,
                SecondTagId = _relation12.SecondTag.Id,
                Name = "changedName"
            };
            var expectedColor = "oldColor";
            _relation12.ArrowStyle = expectedColor;
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(_admin))
                    .WithSet<AnnotationTag>(db => db.AddRange(_tag1, _tag2))
                    .WithSet<AnnotationTagRelation>(db => db.Add(_relation12))
                )
                .Calling(c => c.PutTagRelation(_relation12.FirstTagId, _relation12.SecondTagId, expected))
                .ShouldHave()
                .DbContext(db => db.WithSet<AnnotationTagRelation>(relations =>
                    relations.Any(actual =>
                        actual.FirstTagId == expected.FirstTagId &&
                        actual.SecondTagId == expected.SecondTagId &&
                        actual.Name == expected.Name &&
                        actual.Color == expectedColor // color should not change as it was not set in the model
                    )
                ))
                .AndAlso()
                .ShouldReturn()
                .Ok();
        }

        /// <summary>
        /// Should return 400 for relations that do not exist
        /// </summary>
       // TODO [Test]
        public void PutTagRelationTest400()
        {
            var model = new AnnotationTagRelationFormModel()
            {
                FirstTagId = _relation12.FirstTag.Id,
                SecondTagId = _relation12.SecondTag.Id,
                Name = "changedName"
            };
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(_admin))
                    .WithSet<AnnotationTag>(db => db.AddRange(_tag1, _tag2))
                )
                .Calling(c => c.PutTagRelation(_relation12.FirstTagId, _relation12.SecondTagId, model))
                .ShouldReturn()
                .BadRequest();
        }

        /// <summary>
        /// Should return 403 for users with the student role
        /// </summary>
        // TODO [Test]
        public void PutTagRelationTest403()
        {
            var model = new AnnotationTagRelationFormModel()
            {
                FirstTagId = _relation12.FirstTag.Id,
                SecondTagId = _relation12.SecondTag.Id,
                Name = "changedName"
            };
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "2"))
                .WithDbContext(dbContext => dbContext.WithSet<User>(db => db.Add(_student)))
                .Calling(c => c.PutTagRelation(_relation12.FirstTagId, _relation12.SecondTagId, model))
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
            var model = new AnnotationTagRelationFormModel()
            {
                FirstTagId = _relation12.FirstTag.Id,
                SecondTagId = _relation12.SecondTag.Id
            };
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(_admin))
                    .WithSet<AnnotationTag>(db => db.AddRange(_tag1, _tag2))
                    .WithSet<AnnotationTagRelation>(db => db.Add(_relation12))
                )
                .Calling(c => c.DeleteTagRelation(model))
                .ShouldHave()
                .DbContext(db => db.WithSet<AnnotationTagRelation>(rels =>
                    !rels.Any(rel => rel.FirstTagId == model.FirstTagId && rel.SecondTagId == model.SecondTagId))
                )
                .AndAlso()
                .ShouldReturn()
                .Ok();
        }

        /// <summary>
        /// Should return 400 for relations that do not exist
        /// </summary>
        [Test]
        public void DeleteTagRelationTest400()
        {
            var model = new AnnotationTagRelationFormModel()
            {
                FirstTagId = _relation12.FirstTag.Id,
                SecondTagId = _relation12.SecondTag.Id
            };
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(_admin))
                    .WithSet<AnnotationTag>(db => db.AddRange(_tag1, _tag2))
                )
                // --> no TagRelation objects were added to the database
                .Calling(c => c.DeleteTagRelation(model))
                .ShouldReturn()
                .BadRequest();
        }

        /// <summary>
        /// Should return 403 for users with the student role
        /// </summary>
        [Test]
        public void DeleteTagRelationTest403()
        {
            var model = new AnnotationTagRelationFormModel()
            {
                FirstTagId = _relation12.FirstTag.Id,
                SecondTagId = _relation12.SecondTag.Id
            };
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "2"))
                .WithDbContext(dbContext => dbContext.WithSet<User>(db => db.Add(_student)))
                .Calling(c => c.DeleteTagRelation(model))
                .ShouldReturn()
                .Forbid();
        }

        #endregion

    }
}
