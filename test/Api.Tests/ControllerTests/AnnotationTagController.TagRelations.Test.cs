using System;
using System.Collections.Generic;
using System.Linq;
using PaderbornUniversity.SILab.Hip.CmsApi.Controllers;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.AnnotationTag;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity.Annotation;
using MyTested.AspNetCore.Mvc;
using Xunit;
// TODO fix ReSharper
// ReSharper disable AccessToModifiedClosure
// ReSharper disable UnusedVariable
// ReSharper disable CollectionNeverUpdated.Local

namespace PaderbornUniversity.SILab.Hip.CmsApi.Tests.ControllerTests
{
    public class AnnotationTagRelationsControllerTest
    {
        private ControllerTester<AnnotationController> _tester;        

		public AnnotationTagRelationsControllerTest()
        {
            _tester = new ControllerTester<AnnotationController>();            
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
                SourceLayer = _tester.Layer1,
                SourceLayerId = _tester.Layer1.Id,
                TargetLayer = _tester.Layer2,
                TargetLayerId = _tester.Layer2.Id,
                Color = "my-color",
                ArrowStyle = "my-style",
                Title = "my-title",
                Description= "my-description"
            };
            var expected = new List<LayerRelationRule>() { myRelation };
            _tester.TestController()
                .WithDbContext(dbContext => dbContext
                    .WithSet<Layer>(db => db.AddRange(_tester.Layer1, _tester.Layer2))
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
            var expected = _tester.LayerRelationRule;
            var model = new RelationFormModel()
            {
                SourceId = expected.SourceLayerId,
                TargetId = expected.TargetLayerId,
                Color = expected.Color,
                ArrowStyle = expected.ArrowStyle
            };
            _tester.TestControllerWithMockData(_tester.Supervisor.Id)
                .Calling(c => c.PostLayerRelationRuleAsync(model))
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
            var expected = _tester.LayerRelationRule;
            var model = new RelationFormModel()
            {
                SourceId = expected.SourceLayerId,
                TargetId = expected.TargetLayerId,
                Color = expected.Color,
                ArrowStyle = expected.ArrowStyle
            };
            _tester.TestController(_tester.Student.Id)
                .WithDbContext(dbContext => dbContext
                    .WithSet<Layer>(db => db.AddRange(expected.SourceLayer, expected.TargetLayer))
                )
                .Calling(c => c.PostLayerRelationRuleAsync(model))
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
            var expected = new List<RelationResult>() { new RelationResult(_tester.Relation12) };
            _tester.TestControllerWithMockData()
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
            _tester.TestControllerWithMockData()
                .Calling(c => c.GetRelationsForId(_tester.TagInstance1.Id))
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
            var expected = new List<RelationResult>() { new RelationResult(_tester.Relation12) };
            _tester.TestControllerWithMockData()
                .Calling(c => c.GetRelationsForId(_tester.Tag1.Id))
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
            _tester.TestControllerWithMockData()
                .Calling(c => c.GetRelationsForId(_tester.Tag2.Id))
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
            var expected = new List<AnnotationTag>() { _tester.Tag2, _tester.Tag4 };
            _tester.TestController()
                .WithDbContext(dbContext => dbContext
                    .WithSet<AnnotationTag>(db => db.AddRange(_tester.Tag1, _tester.Tag2, _tester.Tag3, _tester.Tag4))
                    .WithSet<Layer>(db => db.AddRange(_tester.Layer1, _tester.Layer2))
                    .WithSet<LayerRelationRule>(db => db.Add(_tester.LayerRelationRule))
                )
                .Calling(c => c.GetAllowedRelationRuleTargetsForTag(_tester.Tag1.Id))
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
            _tester.TestControllerWithMockData()
                // no layer relation rules exist from layer2 to layer1 --> no relations from tag2 to tag1 / tag3 allowed
                .Calling(c => c.GetAllowedRelationRuleTargetsForTag(_tester.Tag2.Id))
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
                .Calling(c => c.GetAllowedRelationRuleTargetsForTag(_tester.Tag1.Id))
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
            var relation35 = new AnnotationTagInstanceRelation(_tester.TagInstance3, tagInstance5) {Id = 5};
            var expected = new List<RelationResult>() { new RelationResult(_tester.Relation34), new RelationResult(relation35) };
            _tester.TestController()
                .WithDbContext(dbContext => dbContext
                    .WithSet<AnnotationTag>(db => db.AddRange(_tester.Tag1, _tester.Tag2, _tester.Tag3, _tester.Tag4))
                    .WithSet<AnnotationTagInstance>(db => db.AddRange(_tester.TagInstance1, _tester.TagInstance2, _tester.TagInstance3, _tester.TagInstance4, tagInstance5))
                    .WithSet<AnnotationTagRelationRule>(db => db.AddRange(_tester.RelationRule12, _tester.RelationRule32, _tester.RelationRule34))
                    .WithSet<AnnotationTagInstanceRelation>(db => db.AddRange(_tester.Relation12, _tester.Relation34, relation35))
                )
                .Calling(c => c.GetAllowedRelationsForInstance(_tester.TagInstance3.Id))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<List<RelationResult>>()
                .Passing(RelationsEqualPredicate(expected));
        }

        /// <summary>
        /// Should return code 200 and an empty list of tag relations if there are no relations possible for the given tag instance
        /// </summary>
        // TODO [Fact]
        //public void GetAvailableRelationsForIdTest_NoRelations()
        //{
        //    var expected = new List<AnnotationTagInstanceRelation>();
        //    var instance3 = new AnnotationTagInstance(_tester.Tag3);
        //    var instances = new List<AnnotationTagInstance>()
        //    {
        //        new AnnotationTagInstance(_tester.Tag1),
        //        new AnnotationTagInstance(_tester.Tag2),
        //        instance3,
        //        new AnnotationTagInstance(_tester.Tag4)
        //    };
        //    _tester.TestControllerWithMockData()
        //        // no relations exist between the tags
        //        // TODO How to model that the tag instances are part of the same document?                
        //        .Calling(c => c.GetAllowedRelationsForInstance(_tester.Tag3.Id))
        //        .ShouldReturn()
        //        .Ok()
        //        .WithModelOfType<List<AnnotationTagInstanceRelation>>()
        //        .Passing(actual => expected.SequenceEqual(actual));
        //}

        /// <summary>
        /// Should return 404 for tags that do not exist
        /// </summary>
        [Fact]
        public void GetAvailableRelationsForIdTest404()
        {
            _tester.TestController()
                .Calling(c => c.GetAllowedRelationsForInstance(_tester.Tag3.Id))
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
                SourceId = _tester.TagInstance1.Id,
                TargetId = _tester.TagInstance2.Id,
                Title = "relationName",
                Color = "schwarzgelb",
                ArrowStyle = "dotted"
            };
            _tester.TestControllerWithMockData()
                .Calling(c => c.PostTagInstanceRelationAsync(expected))
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
                SourceId = _tester.Tag3.Id,
                TargetId = _tester.Tag2.Id,
                Title = "child-to-toplevel-relation"
            };
            _tester.TestController()
                .WithDbContext(dbContext => dbContext
                    .WithSet<AnnotationTag>(db => db.AddRange(_tester.Tag1, _tester.Tag2, _tester.Tag3))
                )
                .Calling(c => c.PostTagInstanceRelationAsync(expected))
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

            // other way around is also not allowed:
            expected = new RelationFormModel()
            {
                SourceId = _tester.Tag2.Id,
                TargetId = _tester.Tag3.Id,
                Title = "toplevel-to-child-relation"
            };
            _tester.TestController()
                .WithDbContext(dbContext => dbContext
                    .WithSet<AnnotationTag>(db => db.AddRange(_tester.Tag1, _tester.Tag2, _tester.Tag3))
                )
                .Calling(c => c.PostTagInstanceRelationAsync(expected))
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
        /// Should return 400 for duplicate tag relations
        /// </summary>
        [Fact]
        public void PostTagInstanceRelationTest_NoDuplicateRelations()
        {
            var expected = new RelationFormModel()
            {
                SourceId = _tester.Relation12.SourceTag.Id,
                TargetId = _tester.Relation12.TargetTag.Id,
                Title = "duplicate-relation"
            };
            _tester.TestController()
                .WithDbContext(dbContext => dbContext
                    .WithSet<AnnotationTagInstance>(db => db.AddRange(_tester.TagInstance1, _tester.TagInstance2))
                    .WithSet<AnnotationTagInstanceRelation>(db => db.Add(_tester.Relation12))
                )
                .Calling(c => c.PostTagInstanceRelationAsync(expected))
                .ShouldReturn()
                .NotFound();
        }

        /// <summary>
        /// Should return 404 for tags that do not exist
        /// </summary>
        [Fact]
        public void PostTagRelationTest404()
        {
            var expected = new RelationFormModel()
            {
                SourceId = _tester.Tag1.Id,
                TargetId = _tester.Tag2.Id,
                Title = "relation-with-nonexisting-tags"
            };
            _tester.TestController()
                // --> tags 1 and 2 were NOT added to the database
                .Calling(c => c.PostTagInstanceRelationAsync(expected))
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
                SourceId = _tester.Tag1.Id,
                TargetId = _tester.Tag2.Id,
                Title = "relation-with-nonexisting-tags"
            };
            _tester.TestController(_tester.Student.Id) // id = 2 --> student
                .Calling(c => c.PostTagInstanceRelationAsync(expected))
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
                SourceId = _tester.Tag1.Id,
                TargetId = _tester.Tag2.Id,
                Title = "relationName",
                Color = "schwarzgelb",
                ArrowStyle = "dotted"
            };
            _tester.TestControllerWithMockData()
                .Calling(c => c.PostTagRelationRuleAsync(expected))
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
            _tester.Relation12.Color = expectedColor;
            var original = RelationFormModelFromRelation(_tester.Relation12);
            var updated = new RelationUpdateModel()
            {
                SourceId = _tester.Relation12.SourceTag.Id,
                TargetId = _tester.Relation12.TargetTag.Id,
                Title = original.Title,
                NewTitle = "changed"

            };
            _tester.TestController()
                .WithDbContext(dbContext => dbContext
                    .WithSet<AnnotationTagInstance>(db => db.AddRange(_tester.TagInstance1, _tester.TagInstance2))
                    .WithSet<AnnotationTagInstanceRelation>(db => db.Add(_tester.Relation12))
                )
                .Calling(c => c.PutTagInstanceRelation(updated))
                .ShouldHave()
                .DbContext(db => db.WithSet<AnnotationTagInstanceRelation>(relations =>
                    relations.Any(actual =>
                        actual.SourceTagId == updated.SourceId &&
                        actual.TargetTagId == updated.TargetId &&
                        actual.Title == updated.Title &&
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
            _tester.TestControllerWithMockData()
                .Calling(c => c.PutTagInstanceRelation(new RelationUpdateModel()))
                .ShouldReturn()
		       	.NotFound();
        }

        /// <summary>
        /// Should return 403 for users with the student role
        /// </summary>
        [Fact]
        public void PutTagRelationTest403()
        {
            var model = RelationFormModelFromRelation(_tester.Relation12);
            var update = new RelationUpdateModel()
            {
                SourceId = _tester.Relation12.SourceTag.Id,
                TargetId = _tester.Relation12.TargetTag.Id,
                Title = model.Title,
                NewTitle = "changed"
            };
            _tester.TestController(_tester.Student.UId) // --> log in as student
                .Calling(c => c.PutTagInstanceRelation(update))
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
            var original = RelationFormModelFromRelationRule(_tester.RelationRule12);
            var update = new RelationUpdateModel()
            {
                SourceId = _tester.RelationRule12.SourceTagId,
                TargetId = _tester.RelationRule12.TargetTagId,
                Title = original.Title,
                Description = "my relation",
                Color = "schwarzgelb",
                ArrowStyle = "dotted"
            };
            _tester.TestControllerWithMockData()
                .Calling(c => c.PutTagRelationRule(update))
                .ShouldHave()
                .DbContext(db => db.WithSet<AnnotationTagRelationRule>(relations =>
                    relations.Any(actual => TagRulesEqual(actual, update))
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
                SourceId = _tester.Relation12.SourceTag.Id,
                TargetId = _tester.Relation12.TargetTag.Id
            };
            _tester.TestController()
                .WithDbContext(dbContext => dbContext
                    .WithSet<AnnotationTag>(db => db.AddRange(_tester.Tag1, _tester.Tag2))
                    .WithSet<AnnotationTagInstanceRelation>(db => db.Add(_tester.Relation12))
                )
                .Calling(c => c.DeleteTagRelationAsync(model))
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
                SourceId = _tester.Relation12.SourceTag.Id,
                TargetId = _tester.Relation12.TargetTag.Id
            };
            _tester.TestControllerWithMockData()
                // --> no AnnotationTagRelation objects were added to the database
                .Calling(c => c.DeleteTagRelationAsync(model))
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
                SourceId = _tester.Relation12.SourceTag.Id,
                TargetId = _tester.Relation12.TargetTag.Id
            };
            _tester.TestController(_tester.Student.Id)
                .Calling(c => c.DeleteTagRelationAsync(model))
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
            var original = RelationFormModelFromRelationRule(_tester.RelationRule12);
            _tester.TestControllerWithMockData()
                .Calling(c => c.DeleteTagRelationRuleAsync(original))
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

        private static bool TagRulesEqual(AnnotationTagRelationRule actual, RelationUpdateModel expected)
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
                var actualArray = actual as RelationResult[] ?? actual.ToArray();
                var expectedArray = expected as RelationResult[] ?? expected.ToArray();
                for (var i = 0; i < actualArray.Count(); i++)
                {
                    var relationResults = expected as RelationResult[] ?? expectedArray.ToArray();
                    if (actualArray.ElementAt(i).SourceId != relationResults.ElementAt(i).SourceId
                        || actualArray.ElementAt(i).TargetId != relationResults.ElementAt(i).TargetId)
                        return false;
                }
                return true;
            };
        }


        #endregion
    }
}
