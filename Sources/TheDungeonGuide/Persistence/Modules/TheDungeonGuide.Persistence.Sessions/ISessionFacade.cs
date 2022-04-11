using Shared.Persistence.Core.Features.Documents.Many;
using TheDungeonGuide.Shared.Features.Sessions;

namespace TheDungeonGuide.Persistence.Sessions;

public interface ISessionFacade : IDocumentFacade<SessionDocument, string, SessionDto>
{
    Task<IEnumerable<SessionDto>> GetAllByGameMasterIdAsync(
        string gameMasterId,
        int?   skip = null,
        int?   take = null);
    Task                          RemovePlayerAsync(string        id, string memberId);
    Task                          RemoveCharacterAsync(string     id, string characterId);
    Task<IEnumerable<SessionDto>> GetAllByCharacterIdAsync(string id, int?   skip = null, int? limit = null);
    Task<IEnumerable<SessionDto>>                        GetAllByMemberId(string         id, int?   skip = null, int? limit = null);
}