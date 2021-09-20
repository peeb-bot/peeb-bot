using System.Threading;
using System.Threading.Tasks;
using Discord.Commands;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using Peeb.Bot.Dtos;
using Peeb.Bot.Modules;
using Peeb.Bot.Pipeline.Commands.SaveCharacter;
using Peeb.Bot.Results.Character;

namespace Peeb.Bot.UnitTests.Modules
{
    [TestFixture]
    [Parallelizable]
    public class CharacterModuleTests : TestBase<CharacterModuleTestsContext>
    {
        [Test]
        public Task IAm_CharacterFound_ShouldReturnResult()
        {
            return TestAsync(
                c => c.IAm(),
                (c, r) => r.Should().NotBeNull().And.BeOfType<IAmResult>().Which.Character.Should().Be(c.Character));
        }
    }

    public class CharacterModuleTestsContext
    {
        public CharacterDto Character { get; set; }
        public Mock<ICommandContext> Context { get; set; }
        public ulong GuildId { get; set; }
        public Mock<IMediator> Mediator { get; set; }
        public CharacterModule Module { get; set; }
        public ulong UserId { get; set; }

        public CharacterModuleTestsContext()
        {
            Character = new CharacterDto("Earth", "Foo", "Bar", "https://foo.bar");
            Context = new Mock<ICommandContext>();
            GuildId = 1;
            Mediator = new Mock<IMediator>();
            UserId = 2;

            Context.SetupGet(c => c.Guild.Id).Returns(GuildId);
            Context.SetupGet(c => c.User.Id).Returns(UserId);

            Mediator
                .Setup(m => m.Send(
                    It.Is<SaveCharacterCommand>(c =>
                        c.GuildId == GuildId &&
                        c.UserId == UserId &&
                        c.World == "Earth" &&
                        c.Forename == "Foo" &&
                        c.Surname == "Bar"),
                    CancellationToken.None))
                .ReturnsAsync(new SaveCharacterCommandResult(Character));

            Module = new CharacterModule(Mediator.Object).Set(m => m.Context, Context.Object);
        }

        public Task<RuntimeResult> IAm()
        {
            return Module.IAm("Earth", "Foo", "Bar");
        }
    }
}
