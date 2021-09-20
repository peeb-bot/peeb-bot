using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Peeb.Bot.Clients.XivApi.Handlers;
using Peeb.Bot.Options;

namespace Peeb.Bot.UnitTests.Clients.XivApi.Handlers
{
    [TestFixture]
    [Parallelizable]
    public class AuthenticationHandlerTests : TestBase<AuthenticationHandlerTestsContext>
    {
        [Test]
        public Task SendAsync_ShouldSetPrivateKeyQueryParam()
        {
            return TestAsync(
                c => c.HttpClient.GetAsync("https://foo.bar"),
                c => c.HttpMessageHandler.Protected().Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri == new Uri("https://foo.bar?private_key=Secret")),
                    ItExpr.IsAny<CancellationToken>()));
        }
    }

    public class AuthenticationHandlerTestsContext
    {
        public AuthenticationHandler Handler { get; set; }
        public HttpClient HttpClient { get; set; }
        public Mock<HttpMessageHandler> HttpMessageHandler { get; set; }
        public Mock<IOptionsMonitor<XivApiOptions>> OptionsMonitor { get; set; }
        public XivApiOptions Settings { get; set; }

        public AuthenticationHandlerTestsContext()
        {
            OptionsMonitor = new Mock<IOptionsMonitor<XivApiOptions>>();
            HttpMessageHandler = new Mock<HttpMessageHandler>();
            Handler = new AuthenticationHandler(OptionsMonitor.Object) { InnerHandler = HttpMessageHandler.Object };
            Settings = new XivApiOptions { Token = "Secret" };

            HttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            OptionsMonitor.SetupGet(m => m.CurrentValue).Returns(Settings);

            HttpClient = new HttpClient(Handler);
        }
    }
}
