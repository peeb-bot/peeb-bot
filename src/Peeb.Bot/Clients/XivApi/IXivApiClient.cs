using System.Threading.Tasks;
using Peeb.Bot.Clients.XivApi.Responses;

namespace Peeb.Bot.Clients.XivApi
{
    public interface IXivApiClient
    {
        Task<CrossWorldLinkshellResponse> GetCrossWorldLinkshell(string id);
        Task<SearchCharactersResponse> SearchCharacters(string server, string name, int page = 1);
    }
}
