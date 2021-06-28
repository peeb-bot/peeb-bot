using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Peeb.Bot.Clients.XivApi.Extensions;

namespace Peeb.Bot.UnitTests.Clients.XivApi.Extensions
{
    [TestFixture]
    [Parallelizable]
    public class HttpClientExtensionsTests : TestBase<HttpClientExtensionsTestsContext>
    {
        [Test]
        public Task GetFromJsonOrDefaultAsync_Ok_ShouldReturnResponse()
        {
            return TestAsync(
                c => c.SetOkResponse(),
                c => c.GetFromJsonOrDefaultAsync(),
                (c, r) => r.Should().NotBeNull().And.BeOfType<StubResponse>().And.BeEquivalentTo(new StubResponse { Value = "Foobar" }));
        }

        [Test]
        public Task GetFromJsonOrDefaultAsync_NotFound_ShouldReturnNull()
        {
            return TestAsync(
                c => c.SetNotFoundResponse(),
                c => c.GetFromJsonOrDefaultAsync(),
                (c, r) => r.Should().BeNull());
        }

        [Test]
        public Task GetFromJsonOrDefaultAsync_InternalServerError_ShouldReturnNull()
        {
            return TestExceptionAsync(
                c => c.SetInternalServerErrorResponse(),
                c => c.GetFromJsonOrDefaultAsync(),
                (c, r) => r.Should().Throw<HttpRequestException>().Which.StatusCode.Should().Be(HttpStatusCode.InternalServerError));
        }
    }

    public class HttpClientExtensionsTestsContext
    {
        public HttpClient HttpClient { get; set; }
        public Mock<HttpMessageHandler> HttpMessageHandler { get; set; }
        public HttpResponseMessage HttpResponseMessage { get; set; }

        public HttpClientExtensionsTestsContext()
        {
            HttpMessageHandler = new Mock<HttpMessageHandler>();
            HttpResponseMessage = new HttpResponseMessage();

            HttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(m => m.Method == HttpMethod.Get && m.RequestUri == new Uri("https://foo.bar")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(HttpResponseMessage);

            HttpClient = new HttpClient(HttpMessageHandler.Object);
        }

        public Task<StubResponse> GetFromJsonOrDefaultAsync()
        {
            return HttpClient.GetFromJsonOrDefaultAsync<StubResponse>("https://foo.bar");
        }

        public HttpClientExtensionsTestsContext SetOkResponse()
        {
            HttpResponseMessage.StatusCode = HttpStatusCode.OK;
            HttpResponseMessage.Content = new StringContent("{ \"value\": \"Foobar\" }");

            return this;
        }

        public HttpClientExtensionsTestsContext SetNotFoundResponse()
        {
            HttpResponseMessage.StatusCode = HttpStatusCode.NotFound;

            return this;
        }

        public HttpClientExtensionsTestsContext SetInternalServerErrorResponse()
        {
            HttpResponseMessage.StatusCode = HttpStatusCode.InternalServerError;

            return this;
        }
    }
}
