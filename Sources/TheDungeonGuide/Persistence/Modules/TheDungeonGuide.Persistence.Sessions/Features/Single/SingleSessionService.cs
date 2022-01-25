using AutoMapper;
using Shared.Persistence.Core.Features.Repository;
using Shared.Persistence.Mongo.Features.Database.Documents.Single;
using TheDungeonGuide.Persistence.Sessions.Features.Exceptions;
using TheDungeonGuide.Shared.Features.Sessions;

namespace TheDungeonGuide.Persistence.Sessions.Features.Single;

public class SingleSessionService : MongoSingleDocumentService<SessionDocument, SessionDto>, ISingleSessionService
{
    private readonly IRepository<SessionDocument, string> _repository;

    public SingleSessionService(IRepository<SessionDocument, string> repository, IMapper mapper) : base(repository,
        mapper) => _repository = repository;

    public async Task RemoveMemberAsync(string id, string memberId)
    {
        var maybeMembers = await _repository.GetFieldAsync(x => x.Id == id, x => x.PlayersIds);

        if (maybeMembers is { Count: > 0 } members)
        {
            if (!members.Contains(memberId)) throw new SessionNotFoundException(id);
            members.Remove(memberId);
            await _repository.UpdateSingleAsync(x => x.Id == id, x => x.PlayersIds, members);
        }
        else
        {
            throw new MemberNotFoundException(memberId);
        }
    }

    public async Task RemoveCharacterAsync(string id, string characterId)
    {
        var maybeCharacters = await _repository.GetFieldAsync(x => x.Id == id, x => x.CharactersIds);

        if (maybeCharacters is { Count: > 0 } characters)
        {
            if (!characters.Contains(characterId)) throw new SessionNotFoundException(id);
            characters.Remove(characterId);
            await _repository.UpdateSingleAsync(x => x.Id == id, x => x.CharactersIds, characters);
        }
        else
        {
            throw new CharacterNotFoundException(characterId);
        }
    }
}