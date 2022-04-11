using Shared.Persistence.Core.Features.Documents.Many;
using TheDungeonGuide.Shared.Features.Sessions;

namespace TheDungeonGuide.Persistence.Sessions.Features.Many;

public interface IManySessionsService : IManyDocumentsService<SessionDocument, string, SessionDto>
{
    Task<IEnumerable<SessionDto>> GetAllByMemberIdAsync(string id, int? skip = null, int? limit = null);
}