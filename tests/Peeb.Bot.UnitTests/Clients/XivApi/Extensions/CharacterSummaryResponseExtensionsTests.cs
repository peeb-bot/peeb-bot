using FluentAssertions;
using NUnit.Framework;
using Peeb.Bot.Clients.XivApi.Extensions;
using Peeb.Bot.Clients.XivApi.Responses;

namespace Peeb.Bot.UnitTests.Clients.XivApi.Extensions
{
    [TestFixture]
    [Parallelizable]
    public class CharacterSummaryResponseExtensionsTests : TestBase<CharacterSummaryResponseExtensionsTestsContext>
    {
        [Test]
        public void GetWorld_ShouldReturnWorld()
        {
            Test(
                c => c.Response.GetWorld(),
                (c, r) => r.Should().Be("Earth"));
        }

        [Test]
        public void GetForename_ShouldReturnForename()
        {
            Test(
                c => c.Response.GetForename(),
                (c, r) => r.Should().Be("Foo"));
        }

        [Test]
        public void GetSurname_ShouldReturnSurname()
        {
            Test(
                c => c.Response.GetSurname(),
                (c, r) => r.Should().Be("Bar"));
        }
    }

    public class CharacterSummaryResponseExtensionsTestsContext
    {
        public CharacterSummaryResponse Response { get; set; }

        public CharacterSummaryResponseExtensionsTestsContext()
        {
            Response = new CharacterSummaryResponse
            {
                Name = "Foo Bar",
                Server = "Earth\u00a0(Milky Way)"
            };
        }
    }
}
