using Shared.Persistence.Core.Features.Documents.Many;
using TheDungeonGuide.Shared.Features.Sessions;

namespace TheDungeonGuide.Persistence.Sessions.Features.Many;

public interface IManySessionsService : IManyDocumentsService<SessionDocument, string, SessionDto>
{
}