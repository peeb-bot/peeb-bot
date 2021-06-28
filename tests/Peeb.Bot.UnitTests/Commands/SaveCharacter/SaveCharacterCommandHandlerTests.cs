using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Peeb.Bot.Clients.XivApi;
using Peeb.Bot.Clients.XivApi.Responses;
using Peeb.Bot.Commands.SaveCharacter;
using Peeb.Bot.Data;
using Peeb.Bot.Models;
using Peeb.Bot.Notifications.CharacterSaved;
using Peeb.Bot.Profiles;
using Peeb.Bot.Services;

namespace Peeb.Bot.UnitTests.Commands.SaveCharacter
{
    [TestFixture]
    [Parallelizable]
    public class SaveCharacterCommandHandlerTests : TestBase<SaveCharacterCommandHandlerTestContext>
    {
        [Test]
        public Task Handle_CharacterDoesNotExist_ShouldAddCharacter()
        {
            return TestAsync(
                c => c.SetCharacterSummaryResponse(),
                c => c.Handle(),
                c =>
                {
                    var character = c.GetCharacter();

                    character.Should().NotBeNull();
                    character.UserId.Should().Be(c.Command.UserId);
                    character.GuildId.Should().Be(c.Command.GuildId);
                    character.LodestoneId.Should().Be(c.CharacterSummaryResponse.Id);
                    character.World.Should().Be(c.Command.World);
                    character.Forename.Should().Be(c.Command.Forename);
                    character.Surname.Should().Be(c.Command.Surname);
                    character.AvatarUrl.Should().Be(c.CharacterSummaryResponse.Avatar);
                    character.Created.Should().Be(c.UtcNow);
                    character.Updated.Should().BeNull();
                });
        }

        [Test]
        public Task Handle_CharacterDoesNotExist_ShouldPublishNotification()
        {
            return TestAsync(
                c => c.SetCharacterSummaryResponse(),
                c => c.Handle(),
                c =>
                {
                    var character = c.GetCharacter();
                    var notificationSource = (INotificationSource)character;

                    notificationSource.Flush().OfType<CharacterSavedNotification>().Should().ContainSingle(n =>
                        n.Id == character.Id &&
                        n.UserId == character.UserId &&
                        n.GuildId == character.GuildId &&
                        n.LodestoneId == character.LodestoneId &&
                        n.World == character.World &&
                        n.Forename == character.Forename &&
                        n.Surname == character.Surname &&
                        n.AvatarUrl == character.AvatarUrl &&
                        n.Created == character.Created &&
                        n.Updated == character.Updated);
                });
        }

        [Test]
        public Task Handle_CharacterExists_ShouldUpdateCharacter()
        {
            return TestAsync(
                c => c.SetCharacter(c.UtcNow.AddDays(-1)).SetCharacterSummaryResponse(),
                c => c.Handle(),
                c =>
                {
                    var character = c.GetCharacter();

                    character.Should().NotBeNull();
                    character.UserId.Should().Be(c.Command.UserId);
                    character.GuildId.Should().Be(c.Command.GuildId);
                    character.LodestoneId.Should().Be(c.CharacterSummaryResponse.Id);
                    character.World.Should().Be(c.Command.World);
                    character.Forename.Should().Be(c.Command.Forename);
                    character.Surname.Should().Be(c.Command.Surname);
                    character.AvatarUrl.Should().Be(c.CharacterSummaryResponse.Avatar);
                    character.Created.Should().Be(c.UtcNow.AddDays(-1));
                    character.Updated.Should().Be(c.UtcNow);
                });
        }

        [Test]
        public Task Handle_CharacterAlreadyExists_ShouldPublishNotification()
        {
            return TestAsync(
                c => c.SetCharacter(c.UtcNow.AddDays(-1)).SetCharacterSummaryResponse(),
                c => c.Handle(),
                c =>
                {
                    var character = c.GetCharacter();
                    var notificationSource = (INotificationSource)character;

                    notificationSource.Flush().OfType<CharacterSavedNotification>().Should().ContainSingle(n =>
                        n.Id == character.Id &&
                        n.UserId == character.UserId &&
                        n.GuildId == character.GuildId &&
                        n.LodestoneId == character.LodestoneId &&
                        n.World == character.World &&
                        n.Forename == character.Forename &&
                        n.Surname == character.Surname &&
                        n.AvatarUrl == character.AvatarUrl &&
                        n.Created == character.Created &&
                        n.Updated == character.Updated);
                });
        }

        [Test]
        public Task Handle_ShouldReturnResult()
        {
            return TestAsync(
                c => c.SetCharacterSummaryResponse(),
                c => c.Handle(),
                (c, r) =>
                {
                    r.Character.Should().NotBeNull();
                    r.Character.World.Should().Be(c.Command.World);
                    r.Character.Forename.Should().Be(c.Command.Forename);
                    r.Character.Surname.Should().Be(c.Command.Surname);
                    r.Character.AvatarUrl.Should().Be(c.CharacterSummaryResponse.Avatar);
                });
        }

        [Test]
        public Task Handle_CharacterNotFound_ShouldReturnEmptyResult()
        {
            return TestAsync(
                c => c.Handle(),
                (c, r) => r.Should().NotBeNull().And.Match<SaveCharacterCommandResult>(cr => cr.Character == null));
        }
    }

    public class SaveCharacterCommandHandlerTestContext
    {
        public CharacterSummaryResponse CharacterSummaryResponse { get; set; }
        public SaveCharacterCommand Command { get; set; }
        public Mock<IDateTimeOffsetService> DateTimeOffsetService { get; set; }
        public PeebDbContext Db { get; set; }
        public SaveCharacterCommandHandler Handler { get; set; }
        public IMapper Mapper { get; set; }
        public SearchCharactersResponse SearchCharactersResponse { get; set; }
        public User User { get; set; }
        public DateTimeOffset UtcNow { get; set; }
        public Mock<IXivApiClient> XivApiClient { get; set; }

        public SaveCharacterCommandHandlerTestContext()
        {
            Command = new SaveCharacterCommand(1, 2, "Earth", "Foo", "Bar");
            DateTimeOffsetService = new Mock<IDateTimeOffsetService>();
            Db = new PeebDbContext(new DbContextOptionsBuilder<PeebDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            Mapper = new MapperConfiguration(c => c.AddProfile<CharacterProfile>()).CreateMapper();
            SearchCharactersResponse = new SearchCharactersResponse();
            UtcNow = DateTimeOffset.UtcNow;
            User = new User(1, UtcNow);
            XivApiClient = new Mock<IXivApiClient>();

            DateTimeOffsetService.SetupGet(s => s.UtcNow).Returns(UtcNow);
            Db.Users.Add(User);
            Db.SaveChanges();
            XivApiClient.Setup(c => c.SearchCharacters(Command.World, $"{Command.Forename} {Command.Surname}", 1)).ReturnsAsync(SearchCharactersResponse);

            Handler = new SaveCharacterCommandHandler(DateTimeOffsetService.Object, Mapper, XivApiClient.Object, Db);
        }

        public Character GetCharacter()
        {
            return Db.Characters.Local.SingleOrDefault();
        }

        public SaveCharacterCommandHandlerTestContext SetCharacter(DateTimeOffset created)
        {
            Db.Characters.Add(new Character(User, Command.GuildId, 4, "Mars", "Bar", "Foo", "https://bar.foo", created));
            Db.SaveChanges();

            return this;
        }

        public SaveCharacterCommandHandlerTestContext SetCharacterSummaryResponse()
        {
            CharacterSummaryResponse = new CharacterSummaryResponse
            {
                Id = 3,
                Server = "Earth\u00a0(Milky Way)",
                Name = "Foo Bar",
                Avatar = "https://foo.bar"
            };

            SearchCharactersResponse.Results.Add(CharacterSummaryResponse);

            return this;
        }

        public Task<SaveCharacterCommandResult> Handle()
        {
            return Handler.Handle(Command, CancellationToken.None);
        }
    }
}
