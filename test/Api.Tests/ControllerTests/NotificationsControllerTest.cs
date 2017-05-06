using MyTested.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.CmsApi.Controllers;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Notifications;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Tests.ControllerTests
{
    public class NotificationsControllerTest
    {
        private ControllerTester<NotificationsController> _tester;      
        public List<NotificationResult> ExpectedListOfNotifications { get; set; }

        public NotificationsControllerTest()
        {
            _tester = new ControllerTester<NotificationsController>();
            ExpectedListOfNotifications = new List<NotificationResult>()
            {
                new NotificationResult(_tester.UnreadNotification), // unread
                new NotificationResult(_tester.ReadNotification)  // read (as we are expecting all notifications)
            };
        }

        #region GET

        /// <summary>
        /// Should return ok if all notifications are retrieved
        /// </summary>
        [Fact]
        public void GetAllNotificationsTest200()
        {
            _tester.TestControllerWithMockData(_tester.Student.Email)                
                .Calling(c => c.GetAllNotifications())
                .ShouldReturn()
                .Ok()
                .WithModelOfType<List<NotificationResult>>()
                .Passing(actual => actual.Count == ExpectedListOfNotifications.Count); // we have 2 notifications i.e. 2 == 2
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
                .NotFound();
        }

        /// <summary>
        /// Should return ok if all unread notifications are retrieved
        /// </summary>
        [Fact]
        public void GetUnreadNotificationsTest200()
        {
            var expected = new List<NotificationResult>()
            {
                new NotificationResult(_tester.UnreadNotification) // we expect only unread notification
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
                .NotFound();
        }

        /// <summary>
        /// Should return ok if notifications count is retrieved correctly
        /// </summary>
        [Fact]
        public void GetNotificationCountTest200()
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            _tester.TestControllerWithMockData(_tester.Student.Email)
                .Calling(c => c.GetNotificationCount())
                .ShouldReturn()
                .Ok()
            .WithModelOfType<int>()
            .AndAlso()
            .Equals(ExpectedListOfNotifications.Count); // We are able to get the expected notification count
        }

        /// <summary>
        /// Should return ok if subscriptions are retrieved correctly
        /// </summary>
        [Fact]
        public void GetSubscriptionsTest200()
        {
            _tester.TestControllerWithMockData(_tester.Student.Email)
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
        public void PostNotificationAsReadTest200()
        {
            _tester.TestControllerWithMockData()                
                .Calling(c => c.Post(_tester.UnreadNotification.NotificationId))
                .ShouldHave()
                .DbContext(db => db.WithSet<Notification>
                    (n => n.Single(not => not.NotificationId == _tester.UnreadNotification.NotificationId).IsRead))
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
                .Calling(c => c.Post(_tester.ReadNotification.NotificationId))
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
            _tester.TestControllerWithMockData(_tester.Student.Email) // Student is already subscribed to this notification                
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
            _tester.TestController(_tester.Supervisor.Email) // Supervisor wants to get subscribed to notification one                
                .Calling(c => c.PutSubscribe(NotificationType.TOPIC_ASSIGNED_TO.ToString())) //This test will add the subscription to the user
                .ShouldHave()
                .DbContext(db => db.WithSet<Subscription>
                    (s => s.Single(not => not.SubscriberId == _tester.Supervisor.Id)))
                .AndAlso()
                .ShouldReturn()
                .Ok();
        }

        /// <summary>
        /// Should return 404 when an invalid subscription is given
        /// </summary>
        [Fact]
        public void PutSubscribeTest404()
        {
            _tester.TestController(_tester.Student.Email)
                .Calling(c => c.PutSubscribe("BadSubscription"))
                .ShouldReturn()
                .BadRequest(); //Returns 404
        }

        /// <summary>
        /// Should return 404 when the user does not have rights
        /// </summary>
        [Fact]
        public void PutSubscribeTest404WhenTryingToChangeSubscription()
        {
            _tester.TestControllerWithMockData(_tester.Student.Email)
                .Calling(c => c.PutSubscribe(NotificationType.TOPIC_CREATED.ToString())) //When trying to change the subscription
                .ShouldReturn()
                .BadRequest(); //Returns 404
        }

        /// <summary>
        /// Should return ok if subscriptions are unsubscribed
        /// </summary>
        [Fact]
        public void PutUnsubscribeTest200()
        {
            _tester.TestControllerWithMockData(_tester.Student.Email) // Student is already subscribed to this notification                
                .Calling(c => c.PutUnsubscribe(NotificationType.TOPIC_ASSIGNED_TO.ToString()))
                .ShouldHave()                
                .DbContext(db => db.WithSet<User>(s => s.Any(actual => actual.Subscriptions.Count == 0)))
                .AndAlso() // Student is no longer subscribed
                .ShouldReturn()
                .Ok(); 
        }        

        /// <summary>
        /// Should return 404 when an invalid subscription is given
        /// </summary>
        [Fact]
        public void PutUnsubscribeTest404()
        {
            _tester.TestController(_tester.Student.Email)                                    
                .Calling(c => c.PutUnsubscribe("BadSubscription"))
                .ShouldReturn()
                .BadRequest(); //Returns 404
        }

        #endregion
    }
}