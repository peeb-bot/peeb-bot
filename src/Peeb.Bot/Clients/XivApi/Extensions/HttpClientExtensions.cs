using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Peeb.Bot.Clients.XivApi.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<T> GetFromJsonOrDefaultAsync<T>(this HttpClient httpClient, string requestUri, CancellationToken cancellationToken = default)
        {
            T value;

            try
            {
                value = await httpClient.GetFromJsonAsync<T>(requestUri, cancellationToken);
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == HttpStatusCode.NotFound)
                {
                    value = default;
                }
                else
                {
                    throw;
                }
            }

            return value;
        }
    }
}
