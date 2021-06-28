using System.Collections.Generic;

namespace Peeb.Bot.Clients.XivApi.Responses
{
    public class CrossworldLinkshellDetailResponse
    {
        public string Id { get; set; }
        public PaginationResponse Pagination { get; set; }
        public CrossWorldLinkshellProfileResponse Profile { get; set; }
        public List<CharacterSummaryResponse> Results { get; set; }
    }
}
