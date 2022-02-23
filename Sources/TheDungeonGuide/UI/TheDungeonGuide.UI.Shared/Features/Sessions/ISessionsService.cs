using DynamicData;
using TheDungeonGuide.Shared.Features.Sessions;

namespace TheDungeonGuide.UI.Shared.Features.Sessions;

public interface ISessionsService
{
    Task<IEnumerable<SessionDto>> GetSessionsByUserId(string userId);
}