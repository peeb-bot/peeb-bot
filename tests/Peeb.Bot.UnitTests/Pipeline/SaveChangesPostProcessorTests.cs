using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using Peeb.Bot.Models;
using Peeb.Bot.Pipeline;

namespace Peeb.Bot.UnitTests.Pipeline
{
    [TestFixture]
    [Parallelizable]
    public class SaveChangesPostProcessorTests : TestBase<SaveChangesPostProcessorTestsContext>
    {
        [Test]
        public Task Process_EntitiesAddedModifiedDeleted_ShouldSaveChanges()
        {
            return TestAsync(
                c => c.AddModifyDeleteEntities(),
                c => c.Process(),
                c => c.ChangesSaved.Should().Be(1));
        }

        [Test]
        public Task Process_EntitiesAddedModifiedDeleted_ShouldPublishNotifications()
        {
            return TestAsync(
                c => c.AddModifyDeleteEntities(),
                c => c.Process(),
                c =>
                {
                    c.Mediator.Verify(m => m.Publish<INotification>(It.IsAny<StubEntityAddedNotification>(), CancellationToken.None), Times.Once);
                    c.Mediator.Verify(m => m.Publish<INotification>(It.IsAny<StubEntityModifiedNotification>(), CancellationToken.None), Times.Once);
                    c.Mediator.Verify(m => m.Publish<INotification>(It.IsAny<StubEntityDeletedNotification>(), CancellationToken.None), Times.Once);
                });
        }

        [Test]
        public Task Process_NotificationsPublishedAndEntitiesAddedModifiedDeletedSubsequently_ShouldSaveChanges()
        {
            return TestAsync(
                c => c.AddModifyDeleteEntities().AddModifyDeleteEntitiesSubsequently(),
                c => c.Process(),
                c => c.ChangesSaved.Should().Be(2));
        }

        [Test]
        public Task Process_NotificationsPublishedAndEntitiesAddedModifiedDeletedSubsequently_ShouldPublishNotifications()
        {
            return TestAsync(
                c => c.AddModifyDeleteEntities().AddModifyDeleteEntitiesSubsequently().AddModifyDeleteEntitiesSubsequently(),
                c => c.Process(),
                c =>
                {
                    c.Mediator.Verify(m => m.Publish<INotification>(It.IsAny<StubEntityAddedNotification>(), CancellationToken.None), Times.Exactly(3));
                    c.Mediator.Verify(m => m.Publish<INotification>(It.IsAny<StubEntityModifiedNotification>(), CancellationToken.None), Times.Exactly(3));
                    c.Mediator.Verify(m => m.Publish<INotification>(It.IsAny<StubEntityDeletedNotification>(), CancellationToken.None), Times.Exactly(3));
                });
        }
    }

    public class SaveChangesPostProcessorTestsContext
    {
        public int ChangesSaved { get; set; }
        public FakePeebDbContext Db { get; set; }
        public Mock<IMediator> Mediator { get; set; }
        public SaveChangesPostProcessor<DummyCommand, DummyCommandResult> PostProcessor { get; set; }
        public MockSequence Sequence { get; set; }

        public SaveChangesPostProcessorTestsContext()
        {
            Db = new FakePeebDbContext();
            Mediator = new Mock<IMediator>();
            Sequence = new MockSequence();
            PostProcessor = new SaveChangesPostProcessor<DummyCommand, DummyCommandResult>(Mediator.Object, Db);
        }

        public Task Process()
        {
            Db.SavedChanges += (_, _) => ChangesSaved++;

            return PostProcessor.Process(null, null, CancellationToken.None);
        }

        public SaveChangesPostProcessorTestsContext AddModifyDeleteEntities()
        {
            var modify = new StubEntity();
            var delete = new StubEntity();

            Db.Entities.Add(modify);
            Db.Entities.Add(delete);
            Db.SaveChanges();

            foreach (INotificationSource notificationSource in Db.Entities)
            {
                notificationSource.Flush();
            }

            modify.Modify();
            delete.Delete();
            Db.Entities.Remove(delete);
            Db.Entities.Add(new StubEntity());

            return this;
        }

        public SaveChangesPostProcessorTestsContext AddModifyDeleteEntitiesSubsequently()
        {
            Mediator
                .InSequence(Sequence)
                .Setup(m => m.Publish<INotification>(It.IsAny<StubEntityAddedNotification>(), CancellationToken.None))
                .Callback(() => Db.Entities.Add(new StubEntity()));

            Mediator
                .InSequence(Sequence)
                .Setup(m => m.Publish<INotification>(It.IsAny<StubEntityModifiedNotification>(), CancellationToken.None))
                .Callback(() =>
                {
                    var entity = Db.Entities.Single(e => e.State == "Modified");

                    entity.Delete();

                    Db.Entities.Remove(entity);
                });

            Mediator
                .InSequence(Sequence)
                .Setup(m => m.Publish<INotification>(It.IsAny<StubEntityDeletedNotification>(), CancellationToken.None))
                .Callback(() => Db.Entities.Single(e => e.State == "Added").Modify());

            return this;
        }
    }
}
