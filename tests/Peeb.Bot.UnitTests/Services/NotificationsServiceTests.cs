using System;
using System.Collections.Generic;
using FluentAssertions;
using MediatR;
using NUnit.Framework;
using Peeb.Bot.Services;

namespace Peeb.Bot.UnitTests.Services
{
    [TestFixture]
    [Parallelizable]
    public class NotificationsServiceTests : TestBase<NotificationsServiceTestsContext>
    {
        [Test]
        public void GetNotification_NotificationAdded_ShouldReturnAddedNotification()
        {
            Test(
                c => c.AddNotifications(),
                c => c.Service.GetNotification<StubEntityAddedNotification>(),
                (c, r) => r.Should().Be(c.Notifications[0]));
        }

        [Test]
        public void GetNotification_NoNotificationAdded_ShouldReturnNull()
        {
            Test(
                c => c.Service.GetNotification<StubEntityAddedNotification>(),
                (c, r) => r.Should().BeNull());
        }

        [Test]
        public void GetNotification_MultipleNotificationsAdded_ShouldThrowException()
        {
            TestException(
                c => c.AddNotifications(),
                c => c.Service.GetNotification<StubEntityModifiedNotification>(),
                (c, r) => r.Should().Throw<InvalidOperationException>());
        }

        [Test]
        public void GetNotifications_MultipleNotificationsAdded_ShouldReturnAddedNotificationsInOrder()
        {
            Test(
                c => c.AddNotifications(),
                c => c.Service.GetNotifications<StubEntityModifiedNotification>(),
                (c, r) => r.Should().NotBeNull().And.HaveCount(2).And.ContainInOrder(c.Notifications[1], c.Notifications[3]));
        }

        [Test]
        public void GetNotifications_NoNotificationsAdded_ShouldReturnEmptyList()
        {
            Test(
                c => c.Service.GetNotifications<StubEntityModifiedNotification>(),
                (c, r) => r.Should().NotBeNull().And.BeEmpty());
        }
    }

    public class NotificationsServiceTestsContext
    {
        public List<INotification> Notifications { get; set; }
        public NotificationsService Service { get; set; }

        public NotificationsServiceTestsContext()
        {
            Service = new NotificationsService();
        }

        public NotificationsServiceTestsContext AddNotifications()
        {
            Notifications = new List<INotification>
            {
                new StubEntityAddedNotification(),
                new StubEntityModifiedNotification(),
                new StubEntityDeletedNotification(),
                new StubEntityModifiedNotification()
            };

            Notifications.ForEach(n => Service.AddNotification(n));

            return this;
        }
    }
}
