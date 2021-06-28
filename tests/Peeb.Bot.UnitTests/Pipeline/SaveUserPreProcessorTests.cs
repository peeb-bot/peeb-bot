using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Peeb.Bot.Commands.SaveUser;
using Peeb.Bot.Models;
using Peeb.Bot.Pipeline;
using Peeb.Bot.Services;

namespace Peeb.Bot.UnitTests.Pipeline
{
    [TestFixture]
    [Parallelizable]
    public class SaveUserPreProcessorTests : TestBase<SaveUserPreProcessorTestsContext>
    {
        [Test]
        public Task Process_RequestIsSaveUserCommand_ShouldSaveUser()
        {
            return TestAsync(
                c => c.SetSaveUserCommand(),
                c => c.Process(),
                c => c.Db.Users.SingleOrDefault().Should().NotBeNull().And.Match<User>(u => u.Created == c.UtcNow));
        }

        [Test]
        public Task Process_RequestIsNotSaveUserCommand_ShouldNotSaveUser()
        {
            return TestAsync(
                c => c.SetNonSaveUserCommand(),
                c => c.Process(),
                c => c.Db.Users.SingleOrDefault().Should().BeNull());
        }
    }

    public class SaveUserPreProcessorTestsContext
    {
        public Mock<IDateTimeOffsetService> DateTimeOffsetService { get; set; }
        public FakePeebDbContext Db { get; set; }
        public SaveUserPreProcessor<object> PreProcessor { get; set; }
        public object Request { get; set; }
        public DateTimeOffset UtcNow { get; set; }

        public SaveUserPreProcessorTestsContext()
        {
            DateTimeOffsetService = new Mock<IDateTimeOffsetService>();
            Db = new FakePeebDbContext();
            UtcNow = DateTimeOffset.UtcNow;

            DateTimeOffsetService.SetupGet(s => s.UtcNow).Returns(UtcNow);

            PreProcessor = new SaveUserPreProcessor<object>(DateTimeOffsetService.Object, Db);
        }

        public Task Process()
        {
            return PreProcessor.Process(Request, CancellationToken.None);
        }

        public SaveUserPreProcessorTestsContext SetSaveUserCommand()
        {
            Request = new Mock<SaveUserCommand>(1UL).Object;

            return this;
        }

        public SaveUserPreProcessorTestsContext SetNonSaveUserCommand()
        {
            Request = new object();

            return this;
        }
    }
}
