using MyTested.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.CmsApi.Controllers;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Tests.ControllerTests
{
    public class NotificationsControllerTest
    {
        private ControllerTester<NotificationsController> _tester;        

        public NotificationsControllerTest()
        {
            _tester = new ControllerTester<NotificationsController>();
        }

        #region GET

        /// <summary>
        /// Should return ok if all notifications are retrieved
        /// </summary>
        [Fact]
        public void GetAllNotificationsTest200()
        {
            var expected = new List<NotificationResult>()
            {
                new NotificationResult(_tester.notificationOne), // unread
                new NotificationResult(_tester.notificationTwo)  // read (as we are expecting all notifications)
            };
            _tester.TestControllerWithMockData(_tester.Student.Email)                
                .Calling(c => c.GetAllNotifications())
                .ShouldReturn()
                .Ok()
                .WithModelOfType<List<NotificationResult>>()
                .Passing(actual => actual.Count == expected.Count); // we have 2 notifications i.e. 2 == 2
        }

        /// <summary>
        /// Should return 404 if no notifications are found
        /// </summary>
        [Fact]
        public void GetAllNotificationsTest404()
        {
            _tester.TestController(_tester.Supervisor.Email)
                .Calling(c => c.GetAllNotifications())
                .ShouldReturn()
                .NotFound(); // Bug in original method
        }

        /// <summary>
        /// Should return ok if all unread notifications are retrieved
        /// </summary>
        [Fact]
        public void GetUnreadNotificationsTest200()
        {
            var expected = new List<NotificationResult>()
            {
                new NotificationResult(_tester.notificationOne) // we expect only unread notification
            };
            _tester.TestControllerWithMockData(_tester.Student.Email)
                .Calling(c => c.GetUnreadNotifications())
                .ShouldReturn()
                .Ok()
                .WithModelOfType<List<NotificationResult>>()
                .Passing(actual => actual.Count == expected.Count); // we get only one unread notification i.e. 1 == 1
        }

        /// <summary>
        /// Should return 404 if no unread notifications are found
        /// </summary>
        [Fact]
        public void GetUnreadNotificationsTest404()
        {
            _tester.TestController(_tester.Supervisor.Email)
                .Calling(c => c.GetUnreadNotifications())
                .ShouldReturn()
                .NotFound(); // Bug in original method
        }

        /// <summary>
        /// Should return ok if notifications count is retrieved correctly
        /// </summary>
        [Fact]
        public void GetNotificationCountTest200()
        {
            var expected = new List<NotificationResult>()
            {
                new NotificationResult(_tester.notificationOne), // we expect the count(2) of this expected only
                new NotificationResult(_tester.notificationTwo)
            };
            _tester.TestControllerWithMockData(_tester.Student.Email)
                .Calling(c => c.GetNotificationCount())
                .ShouldReturn()
                .Ok()
                .Equals(expected.Count); // we get 2 as expected
        }

        /// <summary>
        /// Should return ok if subscriptions are retrieved correctly
        /// </summary>
        [Fact]
        public void GetSubscriptionsTest200()
        {
            var subscriptions = new Subscription // Adding a new subscription
            {
                SubscriptionId = 1,
                SubscriberId = _tester.Student.Id,
                Subscriber = _tester.Student,
                Type = NotificationType.TOPIC_ASSIGNED_TO
            };

            _tester.TestControllerWithMockData(_tester.Student.Email)
                .WithDbContext(dbContext => dbContext                    
                    .WithSet<Subscription>(db => db.Add(subscriptions)))
                .Calling(c => c.GetSubscriptions())
                .ShouldReturn()
                .Ok()
                .WithModelOfType<IEnumerable<string>>()
                .Passing(g => g.Count() == 1); // We are able to get the expected subscription of the caller
        }

        /// <summary>
        /// Should return ok if subscriptions are retrieved correctly
        /// </summary>
        [Fact]
        public void GetNotificationsTypesTest200()
        {
            _tester.TestController()
                .Calling(c => c.GetNotificationsTypes())
                .ShouldReturn()
                .Ok()
                .WithModelOfType<List<string>>()
                .Passing(g => g.Count() == 9); // In total we have 9 Notification types and hence 9 is expected
        }

        #endregion


        #region Post

        /// <summary>
        /// Should return ok notification is marked as read
        /// </summary>
        [Fact]
        public void PostTest200()
        {
            _tester.TestControllerWithMockData()                
                .Calling(c => c.Post(_tester.notificationOne.NotificationId))
                .ShouldHave()
                .DbContext(db => db.WithSet<Notification>
                    (n => n.Single(not => not.NotificationId == _tester.notificationOne.NotificationId).IsRead == true))
                .AndAlso() //Checking if notification is marked true
                .ShouldReturn()
                .Ok();
        }

        /// <summary>
        /// Should return 404 when notification is not available
        /// </summary>
        [Fact]
        public void PostTest404ForAlreadymarkedNotification()
        {
            _tester.TestController()                
                .Calling(c => c.Post(_tester.notificationTwo.NotificationId))
                .ShouldReturn()
                .NotFound(); //Returns 404
        }

        #endregion

        #region Put

        /// <summary>
        /// Should return ok if subscriptions are updated correctly
        /// </summary>
        [Fact]
        public void PutSubscribeTest200ForUpdating()
        {
            var subscriptions = new Subscription // Adding a new subscription
            {
                SubscriptionId = 1,
                SubscriberId = _tester.Student.Id,
                Subscriber = _tester.Student,
                Type = NotificationType.TOPIC_ASSIGNED_TO
            };

            _tester.TestControllerWithMockData(_tester.Student.Email) // Student is already subscribed to this notification
                .WithDbContext(dbContext => dbContext                    
                    .WithSet<Subscription>(db => db.Add(subscriptions)))
                .Calling(c => c.PutSubscribe(NotificationType.TOPIC_ASSIGNED_TO.ToString()))
                .ShouldHave()
                .DbContext(db => db.WithSet<Subscription>
                    (s => s.Single(not => not.SubscriberId == _tester.Student.Id)))
                .AndAlso() 
                .ShouldReturn()
                .Ok(); //This test will update the existing subscription to the user
        }

        /// <summary>
        /// Should return ok if subscriptions are added correctly
        /// </summary>
        [Fact]
        public void PutSubscribeTest200ForAdding()
        {
            _tester.TestControllerWithMockData(_tester.Supervisor.Email) // Supervisor wants to get subscribed to notification one                
                .Calling(c => c.PutSubscribe(NotificationType.TOPIC_ASSIGNED_TO.ToString())) //This test will add the subscription to the user
                .ShouldHave()
                .DbContext(db => db.WithSet<Subscription>
                    (s => s.Single(not => not.SubscriberId == _tester.Supervisor.Id)))
                .AndAlso()
                .ShouldReturn()
                .Ok();
        }

        /// <summary>
        /// Should return 403 when giving bad notification type
        /// </summary>
        [Fact]
        public void PutSubscribeTest403()
        {
            _tester.TestController()
                .Calling(c => c.PutSubscribe("BadSubscription"))
                .ShouldReturn()
                .BadRequest(); //Returns 403
        }

        /// <summary>
        /// Should return 403 when notification is not available
        /// </summary>
        [Fact]
        public void PutSubscribeTest403WhenTryingToChangeSubscription()
        {
            var subscriptions = new Subscription 
            {
                SubscriptionId = 1,                
                SubscriberId = _tester.Student.Id, //Student is already subscribed to this notification
                Subscriber = _tester.Student,
                Type = NotificationType.TOPIC_ASSIGNED_TO
            };

            _tester.TestControllerWithMockData(_tester.Student.Email)
                .WithDbContext(dbContext => dbContext                    
                    .WithSet<Subscription>(db => db.Add(subscriptions))) // Adding the subscription to in-memory database
                .Calling(c => c.PutSubscribe(NotificationType.TOPIC_CREATED.ToString())) //When trying to change the subscription
                .ShouldReturn()
                .BadRequest(); //Returns 403
        }

        /// <summary>
        /// Should return ok if subscriptions are unsubscribed
        /// </summary>
        [Fact]
        public void PutUnsubscribeTest200()
        {
            var subscriptions = new Subscription // Adding a new subscription
            {
                SubscriptionId = 1,
                SubscriberId = _tester.Student.Id,
                Subscriber = _tester.Student,
                Type = NotificationType.TOPIC_ASSIGNED_TO
            };

            _tester.TestControllerWithMockData(_tester.Student.Email) // Student is already subscribed to this notification
                .WithDbContext(dbContext => dbContext
                    .WithSet<Subscription>(db => db.Add(subscriptions)))
                .Calling(c => c.PutUnsubscribe(NotificationType.TOPIC_ASSIGNED_TO.ToString()))
                .ShouldHave()                
                .DbContext(db => db.WithSet<User>(s => s.Any(actual => actual.Subscriptions.Count == 0)))
                .AndAlso() // Student is no longer subscribed
                .ShouldReturn()
                .Ok(); 
        }        

        /// <summary>
        /// Should return 403 when giving bad notification type
        /// </summary>
        [Fact]
        public void PutUnsubscribeTest403()
        {
            _tester.TestController()                                    
                .Calling(c => c.PutUnsubscribe("BadSubscription"))
                .ShouldReturn()
                .BadRequest(); //Returns 403
        }

        /// <summary>
        /// Should return 403 when notification is not available
        /// </summary>
        [Fact]
        public void PutUnsubscribeTest403WhenTryingToUnsubscribeWithWrongType()
        {
            var subscriptions = new Subscription
            {
                SubscriptionId = 1,
                SubscriberId = _tester.Student.Id, //Student is already subscribed to this notification
                Subscriber = _tester.Student,
                Type = NotificationType.TOPIC_ASSIGNED_TO
            };

            _tester.TestControllerWithMockData(_tester.Student.Email)
                .WithDbContext(dbContext => dbContext
                    .WithSet<Subscription>(db => db.Add(subscriptions))) // Adding the subscription to in-memory database
                .Calling(c => c.PutSubscribe(NotificationType.TOPIC_CREATED.ToString())) //When trying to unsubscribe with the wrong type
                .ShouldReturn()
                .BadRequest(); //Returns 403
        }

        #endregion
    }
}