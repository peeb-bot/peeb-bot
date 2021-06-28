using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Peeb.Bot.Clients.XivApi;
using Peeb.Bot.Clients.XivApi.Extensions;
using Peeb.Bot.Data;
using Peeb.Bot.Dtos;
using Peeb.Bot.Services;

namespace Peeb.Bot.Commands.SaveCharacter
{
    public class SaveCharacterCommandHandler : IRequestHandler<SaveCharacterCommand, SaveCharacterCommandResult>
    {
        private readonly IDateTimeOffsetService _dateTimeOffsetService;
        private readonly IMapper _mapper;
        private readonly IXivApiClient _xivApiClient;
        private readonly PeebDbContext _db;

        public SaveCharacterCommandHandler(IDateTimeOffsetService dateTimeOffsetService, IMapper mapper, IXivApiClient xivApiClient, PeebDbContext db)
        {
            _dateTimeOffsetService = dateTimeOffsetService;
            _mapper = mapper;
            _xivApiClient = xivApiClient;
            _db = db;
        }

        public async Task<SaveCharacterCommandResult> Handle(SaveCharacterCommand request, CancellationToken cancellationToken)
        {
            var searchCharactersResponse = await _xivApiClient.SearchCharacters(request.World, $"{request.Forename} {request.Surname}");

            if (searchCharactersResponse.Results.Count != 1)
            {
                return new SaveCharacterCommandResult();
            }

            var characterSummaryResponse = searchCharactersResponse.Results.Single();
            var character = await _db.Characters.SingleOrDefaultAsync(c => c.UserId == request.UserId && c.GuildId == request.GuildId, cancellationToken);

            if (character == null)
            {
                var user = await _db.Users.SingleAsync(u => u.Id == request.UserId, cancellationToken);

                _db.Characters.Add(character = user.AddCharacter(
                    request.GuildId,
                    characterSummaryResponse.Id,
                    characterSummaryResponse.GetWorld(),
                    characterSummaryResponse.GetForename(),
                    characterSummaryResponse.GetSurname(),
                    characterSummaryResponse.Avatar,
                    _dateTimeOffsetService.UtcNow));
            }
            else
            {
                character.Update(
                    characterSummaryResponse.Id,
                    characterSummaryResponse.GetWorld(),
                    characterSummaryResponse.GetForename(),
                    characterSummaryResponse.GetSurname(),
                    characterSummaryResponse.Avatar,
                    _dateTimeOffsetService.UtcNow);
            }

            var characterDto = _mapper.Map<CharacterDto>(character);

            return new SaveCharacterCommandResult(characterDto);
        }
    }
}
