using System.Security.Claims;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity;
using MyTested.AspNetCore.Mvc;
using MyTested.AspNetCore.Mvc.Builders.Contracts.Controllers;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity.Annotation;
using System.Collections.Generic;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Tests
{
    public class ControllerTester<T>
        where T: class
    {
        private readonly User _admin;
        private readonly User _student;
        private readonly User _supervisor;
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

        public ControllerTester()
        {
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
            _tagInstance1 = new AnnotationTagInstance(_tag1) { Id = 1 };
            _tagInstance2 = new AnnotationTagInstance(_tag2) { Id = 2 };
            _tagInstance3 = new AnnotationTagInstance(_tag3) { Id = 3 };
            _tagInstance4 = new AnnotationTagInstance(_tag4) { Id = 4 };
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

        /// <summary>
        /// Use this for bootstrapping your tests.
        /// Adds an admin, student and supervisor user to the database.
        /// </summary>
        /// <param name="userIdentity">The identity (i.e. the email address) of the user as whom you want to make the call. Defaults to admin.</param>
        /// <returns>An instance of IAndControllerBuilder, i.e. you can chain MyTested test method calls to the return value.</returns>
        public IAndControllerBuilder<T> TestController(string userIdentity = "admin@hipapp.de")
        {
            return MyMvc
                .Controller<T>()
                .WithAuthenticatedUser(user => user.WithClaim(ClaimTypes.Name, userIdentity))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.AddRange(_admin, _student, _supervisor))
                );
        }

        public IAndControllerBuilder<T> TestControllerWithMockData(string userIdentity = "admin@hipapp.de")
        {
            return TestController(userIdentity)
                .WithDbContext(dbContext => dbContext                    
                    .WithSet<AnnotationTag>(db => db.AddRange(_tag1, _tag2, _tag3, _tag4))
                    .WithSet<Layer>(db => db.AddRange(_layer1, _layer2))
                    .WithSet<LayerRelationRule>(db => db.Add(_layerRelationRule))
                    .WithSet<AnnotationTagRelationRule>(db => db.AddRange(_relationRule12, _relationRule32, _relationRule34))
                    .WithSet<AnnotationTagInstance>(db => db.AddRange(_tagInstance1, _tagInstance2, _tagInstance3, _tagInstance4))                        
                );
        }
    }
}
