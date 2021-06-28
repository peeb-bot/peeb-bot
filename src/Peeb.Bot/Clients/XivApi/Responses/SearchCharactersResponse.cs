using System.Collections.Generic;

namespace Peeb.Bot.Clients.XivApi.Responses
{
    public class SearchCharactersResponse
    {
        public PaginationResponse Pagination { get; set; }
        public List<CharacterSummaryResponse> Results { get; set; } = new();
    }
}
