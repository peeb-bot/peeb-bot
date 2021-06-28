using AutoMapper;
using NUnit.Framework;
using Peeb.Bot.Profiles;

namespace Peeb.Bot.UnitTests.Profiles
{
    [TestFixture]
    [Parallelizable]
    public class ProfileTests : TestBase<ProfileTestsContext>
    {
        [Test]
        public void MapperConfiguration_ShouldBeValid()
        {
            Test(c => c.MapperConfiguration(), (_, c) => c.AssertConfigurationIsValid());
        }
    }

    public class ProfileTestsContext
    {
        public MapperConfiguration MapperConfiguration()
        {
            return new(c => c.AddMaps(typeof(CharacterProfile)));
        }
    }
}
