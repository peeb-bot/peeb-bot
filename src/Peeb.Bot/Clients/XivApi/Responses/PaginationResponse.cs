namespace Peeb.Bot.Clients.XivApi.Responses
{
    public class PaginationResponse
    {
        public int Page { get; set; }
        public object PageNext { get; set; }
        public object PagePrev { get; set; }
        public int PageTotal { get; set; }
        public int Results { get; set; }
        public int ResultsPerPage { get; set; }
        public int ResultsTotal { get; set; }
    }
}
