using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Microsoft.Extensions.Options;
using Peeb.Bot.Settings;

namespace Peeb.Bot.Clients.XivApi.Handlers
{
    public class AuthenticationHandler : DelegatingHandler
    {
        private readonly IOptionsMonitor<XivApiSettings> _settings;

        public AuthenticationHandler(IOptionsMonitor<XivApiSettings> settings)
        {
            _settings = settings;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.RequestUri = new Url(request.RequestUri).SetQueryParam("private_key", _settings.CurrentValue.Token).ToUri();

            return base.SendAsync(request, cancellationToken);
        }
    }
}
