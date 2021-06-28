using Peeb.Bot.Clients.XivApi.Responses;

namespace Peeb.Bot.Clients.XivApi.Extensions
{
    public static class CharacterSummaryResponseExtensions
    {
        public static string GetWorld(this CharacterSummaryResponse response)
        {
            return response.Server.Split('\u00a0')[0];
        }

        public static string GetForename(this CharacterSummaryResponse response)
        {
            return response.Name.Split(' ')[0];
        }

        public static string GetSurname(this CharacterSummaryResponse response)
        {
            return response.Name.Split(' ')[1];
        }
    }
}
