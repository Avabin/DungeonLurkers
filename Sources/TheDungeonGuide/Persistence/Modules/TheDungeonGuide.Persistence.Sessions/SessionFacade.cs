using Shared.Persistence.Core.Features.Documents.Many;
using TheDungeonGuide.Persistence.Sessions.Features.Many;
using TheDungeonGuide.Persistence.Sessions.Features.Single;
using TheDungeonGuide.Shared.Features.Sessions;

namespace TheDungeonGuide.Persistence.Sessions;

public class SessionFacade : DocumentFacade<SessionDocument, string, SessionDto>, ISessionFacade
{
    private readonly IManySessionsService  _manyDocumentsService;
    private readonly ISingleSessionService _singleDocumentService;

    public SessionFacade(
        ISingleSessionService singleDocumentService,
        IManySessionsService  manyDocumentsService) :
        base(singleDocumentService, manyDocumentsService)
    {
        _singleDocumentService = singleDocumentService;
        _manyDocumentsService  = manyDocumentsService;
    }


    public Task<IEnumerable<SessionDto>> GetAllByGameMasterIdAsync(
        string gameMasterId,
        int?   skip = null,
        int?   take = null)
    {
        return _manyDocumentsService.GetAllByPredicateAsync(x => x.GameMasterId == gameMasterId, skip, take);
    }

    public Task RemovePlayerAsync(string id, string memberId) => _singleDocumentService.RemoveMemberAsync(id, memberId);

    public Task RemoveCharacterAsync(string id, string characterId) =>
        _singleDocumentService.RemoveCharacterAsync(id, characterId);

    public Task<IEnumerable<SessionDto>> GetAllByCharacterIdAsync(
        string id,
        int?   skip  = null,
        int?   limit = null)
    {
        return _manyDocumentsService.GetAllByPredicateAsync(x => x.CharactersIds.Contains(id), skip, limit);
    }
}