using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity;
using MyTested.AspNetCore.Mvc;
using MyTested.AspNetCore.Mvc.Builders.Contracts.Controllers;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity.Annotation;
using System.Collections.Generic;
using System;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Notifications;
using PaderbornUniversity.SILab.Hip.CmsApi.Tests.Utility;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Tests
{
    public class ControllerTester<T>
        where T: class
    {
        public User Admin { get; }
        public User Student { get; }
        public User Supervisor { get; }
        public AnnotationTag Tag1 { get; set; }
        public AnnotationTag Tag2 { get; set; }
        public AnnotationTag Tag3 { get; set; }
        public AnnotationTag Tag4 { get; set; }
        public AnnotationTagInstance TagInstance1 { get; set; }
        public AnnotationTagInstance TagInstance2 { get; set; }
        public AnnotationTagInstance TagInstance3 { get; set; }
        public AnnotationTagInstance TagInstance4 { get; set; }
        public AnnotationTagRelationRule RelationRule12 { get; set; }
        public AnnotationTagRelationRule RelationRule32 { get; set; }
        public AnnotationTagRelationRule RelationRule34 { get; set; }
        public AnnotationTagInstanceRelation Relation12 { get; set; }
        public AnnotationTagInstanceRelation Relation32 { get; set; }
        public AnnotationTagInstanceRelation Relation34 { get; set; }
        public Layer Layer2 { get; set; }
        public Layer Layer1 { get; set; }
        public LayerRelationRule LayerRelationRule { get; set; }
        public Topic TopicOne { get; set; }
        public Topic TopicTwo { get; set; }
        public Notification UnreadNotification { get; set; }
        public Notification ReadNotification { get; set; }
        public Subscription Subscription { get; set; }
        public TopicUser SupervisorUser { get; set; }
        public TopicUser StudentUser { get; set; }
        public Document FirstDocument { get; set; }
        public AnnotationTagInstance TagInstanceForDocument { get; set; }

        public ControllerTester()
        {
            Admin = new User
            {
                Id = 1,
                Email = "admin@hipapp.de",
                Role = "Administrator"
            };
            Student = new User
            {
                Id = 2,
                Email = "student@hipapp.de",
                Role = "Student"
            };
            Supervisor = new User
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
            Layer1 = new Layer() { Id = 1, Name = "Time" };
            Layer2 = new Layer() { Id = 2, Name = "Perspective" };
            Tag1 = new AnnotationTag() { Id = 1, Layer = Layer1.Name };
            Tag2 = new AnnotationTag() { Id = 2, Layer = Layer2.Name };
            Tag3 = new AnnotationTag() { Id = 3, Layer = Layer1.Name };
            Tag4 = new AnnotationTag() { Id = 4, Layer = Layer2.Name };
            Tag1.ChildTags = new List<AnnotationTag>() { Tag3 };
            Tag2.ChildTags = new List<AnnotationTag>() { Tag4 };
            RelationRule12 = new AnnotationTagRelationRule() { Id = 3, SourceTagId = Tag1.Id, TargetTagId = Tag2.Id, Title = "Tag Relation Rule 1->2" };
            RelationRule32 = new AnnotationTagRelationRule() { Id = 5, SourceTagId = Tag3.Id, TargetTagId = Tag2.Id, Title = "Tag Relation Rule 3->2" };
            RelationRule34 = new AnnotationTagRelationRule() { Id = 7, SourceTagId = Tag3.Id, TargetTagId = Tag4.Id, Title = "Tag Relation Rule 3->4" };
            TagInstance1 = new AnnotationTagInstance(Tag1) { Id = 1 };
            TagInstance2 = new AnnotationTagInstance(Tag2) { Id = 2 };
            TagInstance3 = new AnnotationTagInstance(Tag3) { Id = 3 };
            TagInstance4 = new AnnotationTagInstance(Tag4) { Id = 4 };
            Relation12 = new AnnotationTagInstanceRelation(TagInstance1, TagInstance2) { Id = 3 };
            Relation32 = new AnnotationTagInstanceRelation(TagInstance3, TagInstance2) { Id = 5 };
            Relation34 = new AnnotationTagInstanceRelation(TagInstance3, TagInstance4) { Id = 7 };
            LayerRelationRule = new LayerRelationRule()
            {
                Id = 3,
                SourceLayer = Layer1,
                SourceLayerId = Layer1.Id,
                TargetLayer = Layer2,
                TargetLayerId = Layer2.Id,
                Color = "test-color",
                ArrowStyle = "test-style"
            };
            TopicOne = new Topic
            {
                Id = 1,
                Title = "Paderborner Dom",
                Status = "InReview",
                Deadline = new DateTime(2017, 5, 04),
                CreatedById = Supervisor.Id,
                Description = "Church"
            };
            TopicTwo = new Topic
            {
                Id = 2,
                Title = "Westerntor",
                Status = "InProgress",
                Deadline = new DateTime(2017, 4, 18),
                CreatedById = Supervisor.Id,
                Description = "Shopping"
            };
            UnreadNotification = new Notification
            {
                NotificationId = 1,
                UserId = Student.Id,
                UpdaterId = Supervisor.Id,
                Type = NotificationType.TOPIC_ASSIGNED_TO,
                TopicId = TopicOne.Id,
                IsRead = false
            };
            ReadNotification = new Notification
            {
                NotificationId = 2,
                UserId = Student.Id,
                UpdaterId = Supervisor.Id,
                Type = NotificationType.TOPIC_ASSIGNED_TO,
                TopicId = TopicTwo.Id,
                IsRead = true
            };
            Subscription = new Subscription // Adding a new subscription
            {
                SubscriptionId = 1,
                SubscriberId = Student.Id,
                Subscriber = Student,
                Type = NotificationType.TOPIC_ASSIGNED_TO
            };
            SupervisorUser = new TopicUser
            {
                TopicId = TopicOne.Id,
                UserId = Supervisor.Id,
                Role = Supervisor.Role
            };
            StudentUser = new TopicUser
            {
                TopicId = TopicOne.Id,
                UserId = Student.Id,
                Role = Student.Role
            };
            FirstDocument = new Document
            {
                TopicId = TopicOne.Id,
                UpdaterId = Admin.Id,
                Content = "Hello"
            };
            TagInstanceForDocument = new AnnotationTagInstance
            {
                Id = 5,
                TagModelId = Tag1.Id,
                Document = FirstDocument
            };
        }

        /// <summary>
        /// Use this for bootstrapping your tests.
        /// Adds an admin, student and supervisor user to the database.
        /// </summary>
        /// <param name="userIdentity">The identity (i.e. the email address) of the user as whom you want to make the call. Defaults to admin.</param>
        /// <param name="role">The role of the user as whom you want to make the call. Defaults to Administrator.</param>
        /// <returns>An instance of IAndControllerBuilder, i.e. you can chain MyTested test method calls to the return value.</returns>
        public IAndControllerBuilder<T> TestController(string userIdentity = "admin@hipapp.de", string role = "Administrator")
        {
            return MyMvc
                .Controller<T>()
                .WithAuthenticatedUser(user => user.WithClaim(CustomClaims.Sub, userIdentity).WithClaim(CustomClaims.Role, role))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.AddRange(Admin, Student, Supervisor))
                );
        }

        public IAndControllerBuilder<T> TestControllerWithMockData(string userIdentity = "admin@hipapp.de", string role = "Administrator")
        {
            return TestController(userIdentity, role)
                .WithDbContext(dbContext => dbContext
                    .WithSet<Document>(db => db.Add(FirstDocument))
                    .WithSet<AnnotationTag>(db => db.AddRange(Tag1, Tag2, Tag3, Tag4))
                    .WithSet<Layer>(db => db.AddRange(Layer1, Layer2))
                    .WithSet<LayerRelationRule>(db => db.Add(LayerRelationRule))
                    .WithSet<AnnotationTagRelationRule>(db => db.AddRange(RelationRule12, RelationRule32, RelationRule34))
                    .WithSet<AnnotationTagInstance>(db => db.AddRange(TagInstance1, TagInstance2, TagInstance3, TagInstance4, TagInstanceForDocument))
                    .WithSet<Topic>(db => db.AddRange(TopicOne, TopicTwo))
                    .WithSet<Notification>(db => db.AddRange(UnreadNotification, ReadNotification))
                    .WithSet<Subscription>(db => db.Add(Subscription))
                    .WithSet<TopicUser>(db => db.AddRange(SupervisorUser, StudentUser))
                );
        }
    }
}
