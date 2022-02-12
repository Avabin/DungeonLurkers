using Shared.Persistence.Core.Features.Documents.Many;
using TheDungeonGuide.Persistence.Sessions.Features.Many;
using TheDungeonGuide.Persistence.Sessions.Features.Single;
using TheDungeonGuide.Shared.Features.Sessions;

namespace TheDungeonGuide.Persistence.Sessions;

public class SessionFacade : DocumentOperationFacade<SessionDocument, string, SessionDto>, ISessionFacade
{
    private readonly IManySessionsService  _manyManyDocumentsService;
    private readonly ISingleSessionService _singleSingleDocumentService;

    public SessionFacade(
        ISingleSessionService singleSingleDocumentService,
        IManySessionsService  manyManyDocumentsService) :
        base(singleSingleDocumentService, manyManyDocumentsService)
    {
        _singleSingleDocumentService = singleSingleDocumentService;
        _manyManyDocumentsService  = manyManyDocumentsService;
    }


    public Task<IEnumerable<SessionDto>> GetAllByGameMasterIdAsync(
        string gameMasterId,
        int?   skip = null,
        int?   take = null)
    {
        return _manyManyDocumentsService.GetAllByPredicateAsync(x => x.GameMasterId == gameMasterId, skip, take);
    }

    public Task RemovePlayerAsync(string id, string memberId) => _singleSingleDocumentService.RemoveMemberAsync(id, memberId);

    public Task RemoveCharacterAsync(string id, string characterId) =>
        _singleSingleDocumentService.RemoveCharacterAsync(id, characterId);

    public Task<IEnumerable<SessionDto>> GetAllByCharacterIdAsync(
        string id,
        int?   skip  = null,
        int?   limit = null)
    {
        return _manyManyDocumentsService.GetAllByPredicateAsync(x => x.CharactersIds.Contains(id), skip, limit);
    }
}