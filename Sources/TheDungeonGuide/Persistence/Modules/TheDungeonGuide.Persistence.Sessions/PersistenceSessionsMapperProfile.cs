using Shared.Features;
using TheDungeonGuide.Shared.Features.Sessions;

namespace TheDungeonGuide.Persistence.Sessions;

public class PersistenceSessionsMapperProfile
    : DtoMapperProfile<SessionDto, CreateSessionDto, UpdateSessionDto, SessionDocument>
{
}