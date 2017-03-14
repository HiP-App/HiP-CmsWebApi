using System;
using System.Collections.Generic;
using System.Linq;
using Api.Controllers;
using Api.Models.Entity;
using Api.Models.AnnotationTag;
using Api.Models.Entity.Annotation;
using MyTested.AspNetCore.Mvc;
using MyTested.AspNetCore.Mvc.Builders.Contracts.Controllers;
using NUnit.Framework;
using Layer = Api.Models.Entity.Annotation.Layer;

namespace Api.Tests.ControllerTests
{
    [TestFixture]
    public class AnnotationTagRelationsControllerTest
    {
        private ControllerTester<AnnotationController> _tester;
        private User _admin;
        private User _student;
        private User _supervisor;
        private Tag _tag1;
        private Tag _tag2;
        private Tag _tag3;
        private Tag _tag4;
        private TagInstance _tagInstance1;
        private TagInstance _tagInstance2;
        private TagInstance _tagInstance3;
        private TagInstance _tagInstance4;
        private TagRelationRule _relationRule12;
        private TagRelationRule _relationRule32;
        private TagRelationRule _relationRule34;
        private TagRelation _relation12;
        private TagRelation _relation32;
        private TagRelation _relation34;
        private Layer _layer2;
        private Layer _layer1;
        private LayerRelationRule _layerRelationRule;

        [SetUp]
        public void BeforeTest()
        {
            _tester = new ControllerTester<AnnotationController>();
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
            _tag1 = new Tag() { Id = 1, Layer = _layer1.Name };
            _tag2 = new Tag() { Id = 2, Layer = _layer2.Name };
            _tag3 = new Tag() { Id = 3, Layer = _layer1.Name };
            _tag4 = new Tag() { Id = 4, Layer = _layer2.Name };
            _tag1.ChildTags = new List<Tag>() { _tag3 };
            _tag2.ChildTags = new List<Tag>() { _tag4 };
            _relationRule12 = new TagRelationRule() { Id = 3, SourceTagId = _tag1.Id, TargetTagId = _tag2.Id, Title = "Tag Relation Rule 1->2" };
            _relationRule32 = new TagRelationRule() { Id = 5, SourceTagId = _tag3.Id, TargetTagId = _tag2.Id, Title = "Tag Relation Rule 3->2" };
            _relationRule34 = new TagRelationRule() { Id = 7, SourceTagId = _tag3.Id, TargetTagId = _tag4.Id, Title = "Tag Relation Rule 3->4" };
            _tagInstance1 = new TagInstance(_tag1) {Id = 1};
            _tagInstance2 = new TagInstance(_tag2) {Id = 2};
            _tagInstance3 = new TagInstance(_tag3) {Id = 3};
            _tagInstance4 = new TagInstance(_tag4) {Id = 4};
            _relation12 = new TagRelation(_tagInstance1, _tagInstance2) { Id = 3 };
            _relation32 = new TagRelation(_tagInstance3, _tagInstance2) { Id = 5 };
            _relation34 = new TagRelation(_tagInstance3, _tagInstance4) { Id = 7 };
            _layerRelationRule = new LayerRelationRule()
            {
                Id = 3,
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
            _tester.TestController()
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
            var model = new RelationFormModel()
            {
                SourceId = expected.SourceLayerId,
                TargetId = expected.TargetLayerId,
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
            var model = new RelationFormModel()
            {
                SourceId = expected.SourceLayerId,
                TargetId = expected.TargetLayerId,
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
        [Test]
        public void GetRelationsTest()
        {
            var expected = new List<RelationResult>() { new RelationResult(_relation12) };
            _tester.TestController()
                .WithDbContext(dbContext => dbContext.WithSet<TagRelation>(db => db.Add(_relation12)))
                .Calling(c => c.GetRelations())
                .ShouldReturn()
                .Ok()
                .WithModelOfType<List<RelationResult>>()
                .Passing(RelationsEqualPredicate(expected));
        }

        /// <summary>
        /// Should return code 200 and an empty list if no relations are present
        /// </summary>
        [Test]
        public void GetRelationsTest_EmptyList()
        {
            _tester.TestController()
                .Calling(c => c.GetRelations())
                .ShouldReturn()
                .Ok()
                .WithModelOfType<List<RelationResult>>()
                .Passing(actual => actual.Count == 0);
        }

        #endregion

        #region GetRelationsForId

        /// <summary>
        /// Should return code 200 and an empty list of tag relations if called for an existing tag that has no relations
        /// </summary>
        [Test]
        public void GetRelationsForIdWithNoExistingRelationsTest()
        {
            _tester.TestController()
                .WithDbContext(dbContext => dbContext
                    .WithSet<TagInstance>(db => db.AddRange(_tagInstance1, _tagInstance2))
                )
                .Calling(c => c.GetRelationsForId(_tagInstance1.Id))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<List<RelationResult>>()
                .Passing(actual => actual.Count == 0);
        }

        /// <summary>
        /// Should return code 200 and a list of all tag relations if called properly for an existing tag with relations
        /// </summary>
        // TODO [Test]
        public void GetRelationsForIdWithOneExistingRelationTest()
        {
            var expected = new List<TagRelation>() { _relation12 };
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(_admin))
                    .WithSet<Tag>(db => db.AddRange(_tag1, _tag2))
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
        // TODO [Test]
        public void GetRelationsForIdUniDirectionalTest()
        {
            // ReSharper disable once CollectionNeverUpdated.Local
            var expected = new List<TagRelation>();
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(_admin))
                    .WithSet<Tag>(db => db.AddRange(_tag1, _tag2))
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

        #region GetAllowedRelationRuleTargetsForTag

        /// <summary>
        /// Should return code 200 and a list of all tags that relation rules are allowed to if called properly.
        /// Duplicate relations are also allowed --> tag2 is also expected to be in the returned list
        /// </summary>
        [Test]
        public void GetAllowedRelationRulesForTagTest()
        {
            var expected = new List<Tag>() { _tag2, _tag4 };
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(_admin))
                    .WithSet<Tag>(db => db.AddRange(_tag1, _tag2, _tag3, _tag4))
                    .WithSet<Layer>(db => db.AddRange(_layer1, _layer2))
                    .WithSet<LayerRelationRule>(db => db.Add(_layerRelationRule))
                )
                .Calling(c => c.GetAllowedRelationRuleTargetsForTag(_tag1.Id))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<List<Tag>>()
                .Passing(actual => expected.SequenceEqual(actual));
        }        

        /// <summary>
        /// Should return code 200 and an empty list tags if there are no relations possible because
        /// the top-level tags do not have a layer relation rule defined
        /// </summary>
        [Test]
        public void GetAllowedRelationRulesForTagTest_NoToplevelRelation()
        {
            var expected = new List<Tag>();
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(_admin))
                    .WithSet<Tag>(db => db.AddRange(_tag1, _tag2, _tag3, _tag4))
                    .WithSet<Layer>(db => db.AddRange(_layer1, _layer2))
                    .WithSet<LayerRelationRule>(db => db.Add(_layerRelationRule))
                )
                // no layer relation rules exist from layer2 to layer1 --> no relations from tag2 to tag1 / tag3 allowed
                .Calling(c => c.GetAllowedRelationRuleTargetsForTag(_tag2.Id))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<List<Tag>>()
                .Passing(actual => expected.SequenceEqual(actual));
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
                .Calling(c => c.GetAllowedRelationRuleTargetsForTag(_tag1.Id))
                .ShouldReturn()
                .BadRequest();
        }

        #endregion

        #region GetAllowedRelationRuleTargetsForTag

        /// <summary>
        /// Should return code 200 and a list of all tag relations that are available for the given tag instance
        /// </summary>
        [Test]
        public void GetAvailableRelationsForIdTest()
        {
            var tagInstance5 = new TagInstance(new Tag() { Id = 5 }) {Id = 5};
            var relation35 = new TagRelation(_tagInstance3, tagInstance5) {Id = 5};
            var expected = new List<RelationResult>() { new RelationResult(_relation34), new RelationResult(relation35) };
            _tester.TestController()
                .WithDbContext(dbContext => dbContext
                    .WithSet<Tag>(db => db.AddRange(_tag1, _tag2, _tag3, _tag4))
                    .WithSet<TagInstance>(db => db.AddRange(_tagInstance1, _tagInstance2, _tagInstance3, _tagInstance4, tagInstance5))
                    .WithSet<TagRelationRule>(db => db.AddRange(_relationRule12, _relationRule32, _relationRule34))
                    .WithSet<TagRelation>(db => db.AddRange(_relation12, _relation34, relation35))
                )
                .Calling(c => c.GetAllowedRelationsForInstance(_tagInstance3.Id))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<List<RelationResult>>()
                .Passing(RelationsEqualPredicate(expected));
        }

        /// <summary>
        /// Should return code 200 and an empty list of tag relations if there are no relations possible for the given tag instance
        /// </summary>
       // TODO [Test]
        public void GetAvailableRelationsForIdTest_NoRelations()
        {
            var expected = new List<TagRelation>() { };
            var instance3 = new TagInstance(_tag3);
            var instances = new List<TagInstance>()
            {
                new TagInstance(_tag1),
                new TagInstance(_tag2),
                instance3,
                new TagInstance(_tag4)
            };
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(_admin))
                    .WithSet<Tag>(db => db.AddRange(_tag1, _tag2, _tag3, _tag4))
                // no relations exist between the tags
                // TODO How to model that the tag instances are part of the same document?
                )
                .Calling(c => c.GetAllowedRelationsForInstance(_tag3.Id))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<List<TagRelation>>()
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
                .Calling(c => c.GetAllowedRelationsForInstance(_tag3.Id))
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
            var expected = new RelationFormModel()
            {
                SourceId = _tag1.Id,
                TargetId = _tag2.Id,
                Title = "relationName",
                Color = "schwarzgelb",
                ArrowStyle = "dotted"
            };
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(_admin))
                    .WithSet<Tag>(db => db.AddRange(_tag1, _tag2))
                )
                .Calling(c => c.PostTagRelation(expected))
                .ShouldHave()
                .DbContext(db => db.WithSet<TagRelation>(relations =>
                    relations.Any(actual =>
                        actual.SourceTagId == expected.SourceId &&
                        actual.TargetTagId == expected.TargetId &&
                        actual.Title == expected.Title &&
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
            var expected = new RelationFormModel()
            {
                SourceId = _tag3.Id,
                TargetId = _tag2.Id,
                Title = "child-to-toplevel-relation"
            };
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(_admin))
                    .WithSet<Tag>(db => db.AddRange(_tag1, _tag2, _tag3))
                )
                .Calling(c => c.PostTagRelation(expected))
                .ShouldHave()
                .DbContext(db => db.WithSet<TagRelation>(relations =>
                    !relations.Any(actual => // negated --> NO relation like this exists
                        actual.SourceTagId == expected.SourceId &&
                        actual.TargetTagId == expected.TargetId &&
                        actual.Title == expected.Title
                    )
                ))
                .AndAlso()
                .ShouldReturn()
                .BadRequest();

            // other way around is also not allowed:
            expected = new RelationFormModel()
            {
                SourceId = _tag2.Id,
                TargetId = _tag3.Id,
                Title = "toplevel-to-child-relation"
            };
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(_admin))
                    .WithSet<Tag>(db => db.AddRange(_tag1, _tag2, _tag3))
                )
                .Calling(c => c.PostTagRelation(expected))
                .ShouldHave()
                .DbContext(db => db.WithSet<TagRelation>(relations =>
                    !relations.Any(actual => // negated --> NO relation like this exists
                        actual.SourceTagId == expected.SourceId &&
                        actual.TargetTagId == expected.TargetId &&
                        actual.Title == expected.Title
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
            var expected = new RelationFormModel()
            {
                SourceId = _relation12.SourceTag.Id,
                TargetId = _relation12.TargetTag.Id,
                Title = "duplcate-relation"
            };
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(_admin))
                    .WithSet<Tag>(db => db.AddRange(_tag1, _tag2))
                    .WithSet<TagRelation>(db => db.Add(_relation12))
                )
                .Calling(c => c.PostTagRelation(expected))
                .ShouldHave()
                .DbContext(db => db.WithSet<TagRelation>(relations =>
                    relations.Count(actual =>
                        actual.SourceTagId == expected.SourceId &&
                        actual.TargetTagId == expected.TargetId &&
                        actual.Title == expected.Title
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
            var expected = new RelationFormModel()
            {
                SourceId = _tag1.Id,
                TargetId = _tag2.Id,
                Title = "relation-with-nonexisting-tags"
            };
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext.WithSet<User>(db => db.Add(_admin)))
                // --> tags 1 and 2 were NOT added to the database
                .Calling(c => c.PostTagRelation(expected))
                .ShouldHave()
                .DbContext(db => db.WithSet<TagRelation>(relations =>
                    !relations.Any(actual => // negated --> NO relation like this exists
                        actual.SourceTagId == expected.SourceId &&
                        actual.TargetTagId == expected.TargetId &&
                        actual.Title == expected.Title
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
            var expected = new RelationFormModel()
            {
                SourceId = _tag1.Id,
                TargetId = _tag2.Id,
                Title = "relation-with-nonexisting-tags"
            };
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "2"))
                .WithDbContext(dbContext => dbContext.WithSet<User>(db => db.Add(_student)))
                .Calling(c => c.PostTagRelation(expected))
                .ShouldHave()
                .DbContext(db => db.WithSet<TagRelation>(relations =>
                    !relations.Any(actual => // negated --> NO relation like this exists
                        actual.SourceTagId == expected.SourceId &&
                        actual.TargetTagId == expected.TargetId &&
                        actual.Title == expected.Title
                    )
                ))
                .AndAlso()
                .ShouldReturn()
                .Forbid();
        }

        #endregion

        #region PostTagRelationRule

        /// <summary>
        /// Should return code 200 if called with ids of two existing tags that do not have a relation yet
        /// </summary>
        [Test]
        public void PostTagRelationRuleTest()
        {
            var expected = new RelationFormModel()
            {
                SourceId = _tag1.Id,
                TargetId = _tag2.Id,
                Title = "relationName",
                Color = "schwarzgelb",
                ArrowStyle = "dotted"
            };
            _tester.TestController()
                .WithDbContext(dbContext => dbContext
                    .WithSet<Tag>(db => db.AddRange(_tag1, _tag2))
                    .WithSet<Layer>(db => db.AddRange(_layer1, _layer2))
                    .WithSet<LayerRelationRule>(db => db.Add(_layerRelationRule))
                )
                .Calling(c => c.PostTagRelationRule(expected))
                .ShouldHave()
                .DbContext(db => db.WithSet<TagRelationRule>(relations =>
                    relations.Any(actual =>
                        actual.SourceTagId == expected.SourceId &&
                        actual.TargetTagId == expected.TargetId &&
                        actual.Title == expected.Title &&
                        actual.Color == expected.Color &&
                        actual.ArrowStyle == expected.ArrowStyle
                    )
                ))
                .AndAlso()
                .ShouldReturn()
                .Ok();
        }

        #endregion

        #region PutTagRelation

        /// <summary>
        /// Should return code 200 if called with ids of two existing tags that have a relation
        /// </summary>
        // TODO [Test]
        public void PutTagRelationTest()
        {
            var original = RelationFormModelFromRelation(_relation12);
            var expected = new RelationFormModel()
            {
                SourceId = _relation12.SourceTag.Id,
                TargetId = _relation12.TargetTag.Id,
                Title = "changedName"
            };
            var expectedColor = "oldColor";
            _relation12.ArrowStyle = expectedColor;
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(_admin))
                    .WithSet<Tag>(db => db.AddRange(_tag1, _tag2))
                    .WithSet<TagRelation>(db => db.Add(_relation12))
                )
                .Calling(c => c.PutTagRelation(original, expected))
                .ShouldHave()
                .DbContext(db => db.WithSet<TagRelation>(relations =>
                    relations.Any(actual =>
                        actual.SourceTagId == expected.SourceId &&
                        actual.TargetTagId == expected.TargetId &&
                        actual.Title == expected.Title &&
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
            var model = RelationFormModelFromRelation(_relation12);
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(_admin))
                    .WithSet<Tag>(db => db.AddRange(_tag1, _tag2))
                )
                .Calling(c => c.PutTagRelation(model, model))
                .ShouldReturn()
                .BadRequest();
        }

        /// <summary>
        /// Should return 403 for users with the student role
        /// </summary>
        // TODO [Test]
        public void PutTagRelationTest403()
        {
            var model = RelationFormModelFromRelation(_relation12);
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "2"))
                .WithDbContext(dbContext => dbContext.WithSet<User>(db => db.Add(_student)))
                .Calling(c => c.PutTagRelation(model, model))
                .ShouldReturn()
                .Forbid();
        }

        #endregion

        #region PutTagRelationRule

        /// <summary>
        /// Should return code 200 if called with a RelationFormModel describing an existing TagRelationRule
        /// </summary>
        [Test]
        public void PutTagRelationRuleTest()
        {
            var original = RelationFormModelFromRelationRule(_relationRule12);
            var expected = new RelationFormModel()
            {
                SourceId = _relationRule12.SourceTagId,
                TargetId = _relationRule12.TargetTagId,
                Title = "relationName",
                Description = "my relation",
                Color = "schwarzgelb",
                ArrowStyle = "dotted"
            };
            _tester.TestController()
                .WithDbContext(dbContext => dbContext
                    .WithSet<Tag>(db => db.AddRange(_tag1, _tag2))
                    .WithSet<Layer>(db => db.AddRange(_layer1, _layer2))
                    .WithSet<LayerRelationRule>(db => db.Add(_layerRelationRule))
                    .WithSet<TagRelationRule>(db => db.Add(_relationRule12))
                )
                .Calling(c => c.PutTagRelationRule(original, expected))
                .ShouldHave()
                .DbContext(db => db.WithSet<TagRelationRule>(relations =>
                    relations.Any(actual => TagRulesEqual(actual, expected))
                ))
                .AndAlso()
                .ShouldReturn()
                .Ok();
        }

        #endregion

        #region DeleteTagRelation


        /// <summary>
        /// Should return code 200 if called for an existing TagRelation
        /// </summary>
        [Test]
        public void DeleteTagRelationTest()
        {
            var model = new RelationFormModel()
            {
                SourceId = _relation12.SourceTag.Id,
                TargetId = _relation12.TargetTag.Id
            };
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(_admin))
                    .WithSet<Tag>(db => db.AddRange(_tag1, _tag2))
                    .WithSet<TagRelation>(db => db.Add(_relation12))
                )
                .Calling(c => c.DeleteTagRelation(model))
                .ShouldHave()
                .DbContext(db => db.WithSet<TagRelation>(rels =>
                    !rels.Any(rel => rel.SourceTagId == model.SourceId && rel.TargetTagId == model.TargetId))
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
            var model = new RelationFormModel()
            {
                SourceId = _relation12.SourceTag.Id,
                TargetId = _relation12.TargetTag.Id
            };
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(_admin))
                    .WithSet<Tag>(db => db.AddRange(_tag1, _tag2))
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
            var model = new RelationFormModel()
            {
                SourceId = _relation12.SourceTag.Id,
                TargetId = _relation12.TargetTag.Id
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

        #region DeleteTagRelationRule


        /// <summary>
        /// Should return code 200 if called with the RelationFormModel describing an existing TagRelationRule
        /// </summary>
        [Test]
        public void DeleteTagRelationRuleTest()
        {
            var original = RelationFormModelFromRelationRule(_relationRule12);
            _tester.TestController()
                .WithDbContext(dbContext => dbContext
                    .WithSet<Tag>(db => db.AddRange(_tag1, _tag2))
                    .WithSet<Layer>(db => db.AddRange(_layer1, _layer2))
                    .WithSet<LayerRelationRule>(db => db.Add(_layerRelationRule))
                    .WithSet<TagRelationRule>(db => db.Add(_relationRule12))
                )
                .Calling(c => c.DeleteTagRelationRule(original))
                .ShouldHave()
                .DbContext(db => db.WithSet<TagRelationRule>(relations =>
                    !(relations.Any(actual => TagRulesEqual(actual, original)))
                ))
                .AndAlso()
                .ShouldReturn()
                .Ok();
        }

        #endregion

        #region Helper Methods

        private RelationFormModel RelationFormModelFromRelation(TagRelation rel)
        {
            return new RelationFormModel(rel.SourceTagId, rel.TargetTagId, rel);
        }
        private RelationFormModel RelationFormModelFromRelationRule(TagRelationRule rel)
        {
            return new RelationFormModel(rel.SourceTagId, rel.TargetTagId, rel);
        }

        private static bool TagRulesEqual(TagRelationRule actual, RelationFormModel expected)
        {
            return actual.SourceTagId == expected.SourceId &&
                   actual.TargetTagId == expected.TargetId &&
                   actual.Title == expected.Title &&
                   actual.Description == expected.Description &&
                   actual.Color == expected.Color &&
                   actual.ArrowStyle == expected.ArrowStyle;
        }

        private static Func<List<RelationResult>, bool> RelationsEqualPredicate(List<RelationResult> expected)
        {
            return actual =>
            {
                for (var i = 0; i < actual.Capacity; i++)
                {
                    if (actual[i].SourceId != expected[i].SourceId || actual[i].TargetId != expected[i].TargetId)
                        return false;
                }
                return true;
            };
        }


        #endregion
    }
}
