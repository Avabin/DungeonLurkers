using Shared.Persistence.Core.Features.Documents.Single;
using TheDungeonGuide.Shared.Features.Sessions;

namespace TheDungeonGuide.Persistence.Sessions.Features.Single;

public interface ISingleSessionService : ISingleDocumentService<SessionDocument, string, SessionDto>
{
    Task RemoveMemberAsync(string    id, string memberId);
    Task RemoveCharacterAsync(string id, string characterId);
}