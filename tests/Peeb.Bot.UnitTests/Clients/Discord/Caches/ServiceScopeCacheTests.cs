using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Peeb.Bot.Clients.Discord.Caches;

namespace Peeb.Bot.UnitTests.Clients.Discord.Caches
{
    [TestFixture]
    [Parallelizable]
    public class ServiceScopeCacheTests : TestBase<ServiceScopeCacheTestsContext>
    {
        [Test]
        public Task Get_AcrossAsynchronousFlow_ShouldReturnServiceScope()
        {
            return TestAsync(
                c => c.Get(),
                (c, r) => r.Should().Be(c.ServiceScope.Object));
        }
    }

    public class ServiceScopeCacheTestsContext
    {
        public Mock<IServiceScope> ServiceScope { get; set; }
        public ServiceScopeCache ServiceScopeCache { get; set; }

        public ServiceScopeCacheTestsContext()
        {
            ServiceScope = new Mock<IServiceScope>();
            ServiceScopeCache = new ServiceScopeCache();
        }

        public async Task<IServiceScope> Get()
        {
            var serviceScopes = await Task.WhenAll(
                Task.Run(() =>
                {
                    ServiceScopeCache.Set(Mock.Of<IServiceScope>());

                    return Task.Run(() => ServiceScopeCache.Get());
                }),
                Task.Run(() =>
                {
                    ServiceScopeCache.Set(ServiceScope.Object);

                    return Task.Run(() => ServiceScopeCache.Get());
                }),
                Task.Run(() =>
                {
                    ServiceScopeCache.Set(Mock.Of<IServiceScope>());

                    return Task.Run(() => ServiceScopeCache.Get());
                })
            );

            return serviceScopes[1];
        }
    }
}
