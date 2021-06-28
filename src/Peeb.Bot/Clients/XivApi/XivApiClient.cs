using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Flurl;
using Microsoft.Extensions.Options;
using Peeb.Bot.Clients.XivApi.Extensions;
using Peeb.Bot.Clients.XivApi.Responses;
using Peeb.Bot.Settings;

namespace Peeb.Bot.Clients.XivApi
{
    public class XivApiClient : IXivApiClient
    {
        private readonly HttpClient _httpClient;

        public XivApiClient(HttpClient httpClient, IOptionsMonitor<XivApiSettings> settings)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(settings.CurrentValue.Url);
        }

        public Task<CrossWorldLinkshellResponse> GetCrossWorldLinkshell(string id)
        {
            return _httpClient.GetFromJsonOrDefaultAsync<CrossWorldLinkshellResponse>($"linkshell/crossworld/{id}");
        }

        public Task<SearchCharactersResponse> SearchCharacters(string server, string name, int page = 1)
        {
            return _httpClient.GetFromJsonAsync<SearchCharactersResponse>(new Url("character/search").SetQueryParams(new { server, name, page }));
        }
    }
}
