using RestEase;
using Shared.Features.Authentication;

namespace TheDungeonGuide.Shared.Features.Sessions;

public interface ISessionsApi : IAuthenticatedApi
{
    [Get("/sessions")]
    Task<IEnumerable<SessionDto>> GetSessionsAsync([Query] int? skip = null, [Query] int? limit = null);
    [Get("/sessions/{id}")] Task<SessionDto> GetSessionByIdAsync([Path] string id);
    [Get("/sessions/gm/{gameMasterId}")]
    Task<IEnumerable<SessionDto>> GetSessionsByGameMasterIdAsync(
        [Path] string gameMasterId,
        [Query] int? skip = 0,
        [Query] int? limit = 10);
    [Get("/sessions/characters/{characterId}")]
    Task<IEnumerable<SessionDto>> GetSessionsByCharacterIdAsync(
        [Path] string characterId,
        [Query] int? skip = 0,
        [Query] int? limit = 10);
    [Get("/sessions/members/{userId}")]
    Task<IEnumerable<SessionDto>> GetSessionsByUserIdAsync(
        [Path]  string userId
        // [Query] int?    skip  = 0,
        // [Query] int?    limit = 10
        );
    [Delete("/sessions/{id}/players/{playerId}")]
    Task RemovePlayerFromSessionAsync([Path] string id, [Path] string playerId);
    [Delete("/sessions/{id}/characters/{characterId}")]
    Task RemoveCharacterFromSessionAsync([Path] string id, [Path] string characterId);
    [Post("/sessions")] Task<SessionDto> CreateSessionAsync([Body] CreateSessionDto session);
    [Put("/sessions/{id}")] Task<SessionDto> UpdateSessionAsync([Path] string id, [Body] UpdateSessionDto session);
    [Delete("/sessions/{id}")] Task DeleteSessionAsync([Path] string id);
}