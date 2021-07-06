using System;
using System.Collections.Generic;

namespace Peeb.Bot.Models
{
    public class User : Entity
    {
        public ulong Id { get; private set; }
        public DateTimeOffset Created { get; private set; }
        public IReadOnlyList<Character> Characters => _characters;

        private readonly List<Character> _characters = new();

        public User(ulong id, DateTimeOffset utcNow)
        {
            Id = id;
            Created = utcNow;
        }

        public User()
        {
        }

        public Character AddCharacter(ulong guildId, int lodestoneId, string world, string forename, string surname, string avatarUrl, DateTimeOffset utcNow)
        {
            var character = new Character(this, guildId, lodestoneId, world, forename, surname, avatarUrl, utcNow);

            _characters.Add(character);

            return character;
        }
    }
}
