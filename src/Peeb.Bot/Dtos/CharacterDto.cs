namespace Peeb.Bot.Dtos
{
    public class CharacterDto
    {
        public string World { get; private set; }
        public string Forename { get; private set; }
        public string Surname { get; private set; }
        public string AvatarUrl { get; private set; }

        public CharacterDto(string world, string forename, string surname, string avatarUrl)
        {
            World = world;
            Forename = forename;
            Surname = surname;
            AvatarUrl = avatarUrl;
        }
    }
}
