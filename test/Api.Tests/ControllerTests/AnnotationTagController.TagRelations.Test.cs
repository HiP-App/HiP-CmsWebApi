using System;
using System.Collections.Generic;
using System.Linq;
using Api.Controllers;
using Api.Models.Entity;
using Api.Models.AnnotationTag;
using Api.Models.Entity.Annotation;
using MyTested.AspNetCore.Mvc;
using Xunit;
using System.Security.Claims;
// TODO fix ReSharper
// ReSharper disable AccessToModifiedClosure
// ReSharper disable UnusedVariable
// ReSharper disable CollectionNeverUpdated.Local

namespace Api.Tests.ControllerTests
{
    public class AnnotationTagRelationsControllerTest
    {
        private ControllerTester<AnnotationController> _tester;
        private User _admin;
        private User _student;
        private User _supervisor;
        private AnnotationTag _tag1;
        private AnnotationTag _tag2;
        private AnnotationTag _tag3;
        private AnnotationTag _tag4;
        private AnnotationTagInstance _tagInstance1;
        private AnnotationTagInstance _tagInstance2;
        private AnnotationTagInstance _tagInstance3;
        private AnnotationTagInstance _tagInstance4;
        private AnnotationTagRelationRule _relationRule12;
        private AnnotationTagRelationRule _relationRule32;
        private AnnotationTagRelationRule _relationRule34;
        private AnnotationTagInstanceRelation _relation12;
        private AnnotationTagInstanceRelation _relation32;
        private AnnotationTagInstanceRelation _relation34;
        private Layer _layer2;
        private Layer _layer1;
        private LayerRelationRule _layerRelationRule;

		public AnnotationTagRelationsControllerTest()
        {
            _tester = new ControllerTester<AnnotationController>();
            // create some User, AnnotationTag and AnnotationTagRelation objects for mocking the database
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
             * Annotation AnnotationTag Relations:
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
            _relationRule12 = new AnnotationTagRelationRule() { Id = 3, SourceTagId = _tag1.Id, TargetTagId = _tag2.Id, Title = "Tag Relation Rule 1->2" };
            _relationRule32 = new AnnotationTagRelationRule() { Id = 5, SourceTagId = _tag3.Id, TargetTagId = _tag2.Id, Title = "Tag Relation Rule 3->2" };
            _relationRule34 = new AnnotationTagRelationRule() { Id = 7, SourceTagId = _tag3.Id, TargetTagId = _tag4.Id, Title = "Tag Relation Rule 3->4" };
            _tagInstance1 = new AnnotationTagInstance(_tag1) {Id = 1};
            _tagInstance2 = new AnnotationTagInstance(_tag2) {Id = 2};
            _tagInstance3 = new AnnotationTagInstance(_tag3) {Id = 3};
            _tagInstance4 = new AnnotationTagInstance(_tag4) {Id = 4};
            _relation12 = new AnnotationTagInstanceRelation(_tagInstance1, _tagInstance2) { Id = 3 };
            _relation32 = new AnnotationTagInstanceRelation(_tagInstance3, _tagInstance2) { Id = 5 };
            _relation34 = new AnnotationTagInstanceRelation(_tagInstance3, _tagInstance4) { Id = 7 };
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
        [Fact]
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
       [Fact]
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
            _tester.TestController("supervisor@hipapp.de")
                .WithDbContext(dbContext => dbContext
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
        [Fact]
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
            _tester.TestController("student@hipapp.de")
                .WithDbContext(dbContext => dbContext
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
        [Fact]
        public void GetRelationsTest()
        {
            var expected = new List<RelationResult>() { new RelationResult(_relation12) };
            _tester.TestController()
                .WithDbContext(dbContext => dbContext.WithSet<AnnotationTagInstanceRelation>(db => db.Add(_relation12)))
                .Calling(c => c.GetRelations())
                .ShouldReturn()
                .Ok()
                .WithModelOfType<List<RelationResult>>()
                .Passing(RelationsEqualPredicate(expected));
        }

        /// <summary>
        /// Should return code 200 and an empty list if no relations are present
        /// </summary>
        [Fact]
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
        [Fact]
        public void GetRelationsForIdWithNoExistingRelationsTest()
        {
            _tester.TestController()
                .WithDbContext(dbContext => dbContext
                    .WithSet<AnnotationTagInstance>(db => db.AddRange(_tagInstance1, _tagInstance2))
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
       [Fact]
        public void GetRelationsForIdWithOneExistingRelationTest()
        {
            var expected = new List<RelationResult>() { new RelationResult(_relation12) };
            _tester.TestController()
                .WithDbContext(dbContext => dbContext
                    .WithSet<AnnotationTag>(db => db.AddRange(_tag1, _tag2))
                    .WithSet<AnnotationTagInstanceRelation>(db => db.Add(_relation12))
                )
                .Calling(c => c.GetRelationsForId(_tag1.Id))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<List<RelationResult>>()
                .Passing(RelationsEqualPredicate(expected));
        }

        /// <summary>
        /// Relations are uni-directional i.e. tag2 (the tag with the INCOMING relation, but no outgoing relations) should have no relations
        /// </summary>
       [Fact]
        public void GetRelationsForIdUniDirectionalTest()
        {
            // ReSharper disable once CollectionNeverUpdated.Local
            var expected = new List<RelationResult>();
            _tester.TestController()
                .WithDbContext(dbContext => dbContext
                    .WithSet<AnnotationTag>(db => db.AddRange(_tag1, _tag2))
                    .WithSet<AnnotationTagInstanceRelation>(db => db.Add(_relation12))
                )
                .Calling(c => c.GetRelationsForId(_tag2.Id))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<List<RelationResult>>()
                .Passing(RelationsEqualPredicate(expected));
        }

        #endregion

        #region GetAllowedRelationRuleTargetsForTag

        /// <summary>
        /// Should return code 200 and a list of all tags that relation rules are allowed to if called properly.
        /// Duplicate relations are also allowed --> tag2 is also expected to be in the returned list
        /// </summary>
        [Fact]
        public void GetAllowedRelationRulesForTagTest()
        {
            var expected = new List<AnnotationTag>() { _tag2, _tag4 };
            _tester.TestController()
                .WithDbContext(dbContext => dbContext
                    .WithSet<AnnotationTag>(db => db.AddRange(_tag1, _tag2, _tag3, _tag4))
                    .WithSet<Layer>(db => db.AddRange(_layer1, _layer2))
                    .WithSet<LayerRelationRule>(db => db.Add(_layerRelationRule))
                )
                .Calling(c => c.GetAllowedRelationRuleTargetsForTag(_tag1.Id))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<List<AnnotationTag>>()
                .Passing(actual => expected.SequenceEqual(actual));
        }        

        /// <summary>
        /// Should return code 200 and an empty list tags if there are no relations possible because
        /// the top-level tags do not have a layer relation rule defined
        /// </summary>
        [Fact]
        public void GetAllowedRelationRulesForTagTest_NoToplevelRelation()
        {
            var expected = new List<AnnotationTag>();
            _tester.TestController()
                .WithDbContext(dbContext => dbContext
                    .WithSet<AnnotationTag>(db => db.AddRange(_tag1, _tag2, _tag3, _tag4))
                    .WithSet<Layer>(db => db.AddRange(_layer1, _layer2))
                    .WithSet<LayerRelationRule>(db => db.Add(_layerRelationRule))
                )
                // no layer relation rules exist from layer2 to layer1 --> no relations from tag2 to tag1 / tag3 allowed
                .Calling(c => c.GetAllowedRelationRuleTargetsForTag(_tag2.Id))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<List<AnnotationTag>>()
                .Passing(actual => expected.SequenceEqual(actual));
        }

        /// <summary>
        /// Should return 404 for tags that do not exist
        /// </summary>
       [Fact]
        public void GetAllowedRelationRulesForTagTest404()
        {
            _tester.TestController()
                .Calling(c => c.GetAllowedRelationRuleTargetsForTag(_tag1.Id))
                .ShouldReturn()
                .NotFound();
        }

        #endregion

        #region GetAllowedRelationRuleTargetsForTag

        /// <summary>
        /// Should return code 200 and a list of all tag relations that are available for the given tag instance
        /// </summary>
        [Fact]
        public void GetAvailableRelationsForIdTest()
        {
            var tagInstance5 = new AnnotationTagInstance(new AnnotationTag() { Id = 5 }) {Id = 5};
            var relation35 = new AnnotationTagInstanceRelation(_tagInstance3, tagInstance5) {Id = 5};
            var expected = new List<RelationResult>() { new RelationResult(_relation34), new RelationResult(relation35) };
            _tester.TestController()
                .WithDbContext(dbContext => dbContext
                    .WithSet<AnnotationTag>(db => db.AddRange(_tag1, _tag2, _tag3, _tag4))
                    .WithSet<AnnotationTagInstance>(db => db.AddRange(_tagInstance1, _tagInstance2, _tagInstance3, _tagInstance4, tagInstance5))
                    .WithSet<AnnotationTagRelationRule>(db => db.AddRange(_relationRule12, _relationRule32, _relationRule34))
                    .WithSet<AnnotationTagInstanceRelation>(db => db.AddRange(_relation12, _relation34, relation35))
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
       // TODO [Fact]
        public void GetAvailableRelationsForIdTest_NoRelations()
        {
            var expected = new List<AnnotationTagInstanceRelation>() { };
            var instance3 = new AnnotationTagInstance(_tag3);
            var instances = new List<AnnotationTagInstance>()
            {
                new AnnotationTagInstance(_tag1),
                new AnnotationTagInstance(_tag2),
                instance3,
                new AnnotationTagInstance(_tag4)
            };
            _tester.TestController()
                .WithDbContext(dbContext => dbContext
                    .WithSet<AnnotationTag>(db => db.AddRange(_tag1, _tag2, _tag3, _tag4))
                // no relations exist between the tags
                // TODO How to model that the tag instances are part of the same document?
                )
                .Calling(c => c.GetAllowedRelationsForInstance(_tag3.Id))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<List<AnnotationTagInstanceRelation>>()
                .Passing(actual => expected.SequenceEqual(actual));
        }

        /// <summary>
        /// Should return 404 for tags that do not exist
        /// </summary>
        [Fact]
        public void GetAvailableRelationsForIdTest404()
        {
            _tester.TestController()
                .Calling(c => c.GetAllowedRelationsForInstance(_tag3.Id))
                .ShouldReturn()
                .NotFound();
        }

        #endregion

        #region PostTagRelation

        /// <summary>
        /// Should return code 200 if called with ids of two existing tags that do not have a relation yet
        /// </summary>
        [Fact]
        public void PostTagInstanceRelationTest()
        {
            var expected = new RelationFormModel()
            {
                SourceId = _tagInstance1.Id,
                TargetId = _tagInstance2.Id,
                Title = "relationName",
                Color = "schwarzgelb",
                ArrowStyle = "dotted"
            };
            _tester.TestController()
                .WithDbContext(dbContext => dbContext
                    .WithSet<AnnotationTagInstance>(db => db.AddRange(_tagInstance1, _tagInstance2))
                )
                .Calling(c => c.PostTagInstanceRelation(expected))
                .ShouldHave()
                .DbContext(db => db.WithSet<AnnotationTagInstanceRelation>(relations =>
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
        [Fact]
        public void PostTagRelationTest_NoChildToFirstLevelRelation()
        {
            var expected = new RelationFormModel()
            {
                SourceId = _tag3.Id,
                TargetId = _tag2.Id,
                Title = "child-to-toplevel-relation"
            };
            _tester.TestController()
                .WithDbContext(dbContext => dbContext
                    .WithSet<AnnotationTag>(db => db.AddRange(_tag1, _tag2, _tag3))
                )
                .Calling(c => c.PostTagInstanceRelation(expected))
                .ShouldHave()
                .DbContext(db => db.WithSet<AnnotationTagInstanceRelation>(relations =>
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
            _tester.TestController()
                .WithDbContext(dbContext => dbContext
                    .WithSet<AnnotationTag>(db => db.AddRange(_tag1, _tag2, _tag3))
                )
                .Calling(c => c.PostTagInstanceRelation(expected))
                .ShouldHave()
                .DbContext(db => db.WithSet<AnnotationTagInstanceRelation>(relations =>
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
        [Fact]
        public void PostTagInstanceRelationTest_NoDuplicateRelations()
        {
            var expected = new RelationFormModel()
            {
                SourceId = _relation12.SourceTag.Id,
                TargetId = _relation12.TargetTag.Id,
                Title = "duplcate-relation"
            };
            _tester.TestController()
                .WithDbContext(dbContext => dbContext
                    .WithSet<AnnotationTagInstance>(db => db.AddRange(_tagInstance1, _tagInstance2))
                    .WithSet<AnnotationTagInstanceRelation>(db => db.Add(_relation12))
                )
                .Calling(c => c.PostTagInstanceRelation(expected))
                .ShouldReturn()
                .BadRequest();
        }

        /// <summary>
        /// Should return 404 for tags that do not exist
        /// </summary>
        [Fact]
        public void PostTagRelationTest404()
        {
            var expected = new RelationFormModel()
            {
                SourceId = _tag1.Id,
                TargetId = _tag2.Id,
                Title = "relation-with-nonexisting-tags"
            };
            _tester.TestController()
                // --> tags 1 and 2 were NOT added to the database
                .Calling(c => c.PostTagInstanceRelation(expected))
                .ShouldHave()
                .DbContext(db => db.WithSet<AnnotationTagInstanceRelation>(relations =>
                    !relations.Any(actual => // negated --> NO relation like this exists
                        actual.SourceTagId == expected.SourceId &&
                        actual.TargetTagId == expected.TargetId &&
                        actual.Title == expected.Title
                    )
                ))
                .AndAlso()
                .ShouldReturn()
                .NotFound();
        }

        /// <summary>
        /// Should return 403 for users with the student role
        /// </summary>
        [Fact]
        public void PostTagRelationTest403()
        {
            var expected = new RelationFormModel()
            {
                SourceId = _tag1.Id,
                TargetId = _tag2.Id,
                Title = "relation-with-nonexisting-tags"
            };
            _tester.TestController("student@hipapp.de") // id = 2 --> student
                .Calling(c => c.PostTagInstanceRelation(expected))
                .ShouldReturn()
                .Forbid();
        }

        #endregion

        #region PostTagRelationRule

        /// <summary>
        /// Should return code 200 if called with ids of two existing tags that do not have a relation yet
        /// </summary>
        [Fact]
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
                    .WithSet<AnnotationTag>(db => db.AddRange(_tag1, _tag2))
                    .WithSet<Layer>(db => db.AddRange(_layer1, _layer2))
                    .WithSet<LayerRelationRule>(db => db.Add(_layerRelationRule))
                )
                .Calling(c => c.PostTagRelationRule(expected))
                .ShouldHave()
                .DbContext(db => db.WithSet<AnnotationTagRelationRule>(relations =>
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
        [Fact]
        public void PutTagRelationTest()
        {
            var expectedColor = "oldColor";
            _relation12.Color = expectedColor;
            var original = RelationFormModelFromRelation(_relation12);
            var expected = new RelationFormModel()
            {
                SourceId = _relation12.SourceTag.Id,
                TargetId = _relation12.TargetTag.Id,
                Title = "changedName"
            };
            _tester.TestController()
                .WithDbContext(dbContext => dbContext
                    .WithSet<AnnotationTagInstance>(db => db.AddRange(_tagInstance1, _tagInstance2))
                    .WithSet<AnnotationTagInstanceRelation>(db => db.Add(_relation12))
                )
                .Calling(c => c.PutTagInstanceRelation(original, expected))
                .ShouldHave()
                .DbContext(db => db.WithSet<AnnotationTagInstanceRelation>(relations =>
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
        /// Should return 404 for relations that do not exist
        /// </summary>
        [Fact]
        public void PutTagRelationTest404()
        {
            var model = RelationFormModelFromRelation(_relation12);
            _tester.TestController()
                .WithDbContext(dbContext => dbContext
                    .WithSet<AnnotationTag>(db => db.AddRange(_tag1, _tag2))
                )
                .Calling(c => c.PutTagInstanceRelation(model, model))
                .ShouldReturn()
		       	.NotFound();
        }

        /// <summary>
        /// Should return 403 for users with the student role
        /// </summary>
        [Fact]
        public void PutTagRelationTest403()
        {
            var model = RelationFormModelFromRelation(_relation12);
            _tester.TestController("student@hipapp.de") // --> log in as student
                .Calling(c => c.PutTagInstanceRelation(model, model))
                .ShouldReturn()
                .Forbid();
        }

        #endregion

        #region PutTagRelationRule

        /// <summary>
        /// Should return code 200 if called with a RelationFormModel describing an existing TagRelationRule
        /// </summary>
        [Fact]
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
                    .WithSet<AnnotationTag>(db => db.AddRange(_tag1, _tag2))
                    .WithSet<Layer>(db => db.AddRange(_layer1, _layer2))
                    .WithSet<LayerRelationRule>(db => db.Add(_layerRelationRule))
                    .WithSet<AnnotationTagRelationRule>(db => db.Add(_relationRule12))
                )
                .Calling(c => c.PutTagRelationRule(original, expected))
                .ShouldHave()
                .DbContext(db => db.WithSet<AnnotationTagRelationRule>(relations =>
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
        [Fact]
        public void DeleteTagRelationTest()
        {
            var model = new RelationFormModel()
            {
                SourceId = _relation12.SourceTag.Id,
                TargetId = _relation12.TargetTag.Id
            };
            _tester.TestController()
                .WithDbContext(dbContext => dbContext
                    .WithSet<AnnotationTag>(db => db.AddRange(_tag1, _tag2))
                    .WithSet<AnnotationTagInstanceRelation>(db => db.Add(_relation12))
                )
                .Calling(c => c.DeleteTagRelation(model))
                .ShouldHave()
                .DbContext(db => db.WithSet<AnnotationTagInstanceRelation>(rels =>
                    !rels.Any(rel => rel.SourceTagId == model.SourceId && rel.TargetTagId == model.TargetId))
                )
                .AndAlso()
                .ShouldReturn()
                .Ok();
        }

        /// <summary>
        /// Should return 404 for relations that do not exist
        /// </summary>
        [Fact]
        public void DeleteTagRelationTest404()
        {
            var model = new RelationFormModel()
            {
                SourceId = _relation12.SourceTag.Id,
                TargetId = _relation12.TargetTag.Id
            };
            _tester.TestController()
                .WithDbContext(dbContext => dbContext
                    .WithSet<AnnotationTag>(db => db.AddRange(_tag1, _tag2))
                )
                // --> no AnnotationTagRelation objects were added to the database
                .Calling(c => c.DeleteTagRelation(model))
                .ShouldReturn()
                .NotFound();
        }

        /// <summary>
        /// Should return 403 for users with the student role
        /// </summary>
        [Fact]
        public void DeleteTagRelationTest403()
        {
            var model = new RelationFormModel()
            {
                SourceId = _relation12.SourceTag.Id,
                TargetId = _relation12.TargetTag.Id
            };
            _tester.TestController("student@hipapp.de")
                .Calling(c => c.DeleteTagRelation(model))
                .ShouldReturn()
                .Forbid();
        }

        #endregion

        #region DeleteTagRelationRule


        /// <summary>
        /// Should return code 200 if called with the RelationFormModel describing an existing TagRelationRule
        /// </summary>
        [Fact]
        public void DeleteTagRelationRuleTest()
        {
            var original = RelationFormModelFromRelationRule(_relationRule12);
            _tester.TestController()
                .WithDbContext(dbContext => dbContext
                    .WithSet<AnnotationTag>(db => db.AddRange(_tag1, _tag2))
                    .WithSet<Layer>(db => db.AddRange(_layer1, _layer2))
                    .WithSet<LayerRelationRule>(db => db.Add(_layerRelationRule))
                    .WithSet<AnnotationTagRelationRule>(db => db.Add(_relationRule12))
                )
                .Calling(c => c.DeleteTagRelationRule(original))
                .ShouldHave()
                .DbContext(db => db.WithSet<AnnotationTagRelationRule>(relations =>
                    !(relations.Any(actual => TagRulesEqual(actual, original)))
                ))
                .AndAlso()
                .ShouldReturn()
                .Ok();
        }

        #endregion

        #region Helper Methods

        private RelationFormModel RelationFormModelFromRelation(AnnotationTagInstanceRelation rel)
        {
            return new RelationFormModel(rel.SourceTagId, rel.TargetTagId, rel);
        }
        private RelationFormModel RelationFormModelFromRelationRule(AnnotationTagRelationRule rel)
        {
            return new RelationFormModel(rel.SourceTagId, rel.TargetTagId, rel);
        }

        private static bool TagRulesEqual(AnnotationTagRelationRule actual, RelationFormModel expected)
        {
            return actual.SourceTagId == expected.SourceId &&
                   actual.TargetTagId == expected.TargetId &&
                   actual.Title == expected.Title &&
                   actual.Description == expected.Description &&
                   actual.Color == expected.Color &&
                   actual.ArrowStyle == expected.ArrowStyle;
        }

        private static Func<IEnumerable<RelationResult>, bool> RelationsEqualPredicate(IEnumerable<RelationResult> expected)
        {
            return actual =>
            {
                for (var i = 0; i < actual.Count(); i++)
                {
                    if (actual.ElementAt(i).SourceId != expected.ElementAt(i).SourceId
                        || actual.ElementAt(i).TargetId != expected.ElementAt(i).TargetId)
                        return false;
                }
                return true;
            };
        }


        #endregion
    }
}
