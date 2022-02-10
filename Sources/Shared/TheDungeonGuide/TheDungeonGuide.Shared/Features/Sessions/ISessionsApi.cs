using RestEase;
using Shared.Features.Authentication;

namespace TheDungeonGuide.Shared.Features.Sessions;

public interface ISessionsApi : IAuthenticatedApi
{
    [Get("/session")]
    Task<IEnumerable<SessionDto>> GetSessionsAsync([Query] int? skip = null, [Query] int? limit = null);
    [Get("/session/{id}")] Task<SessionDto> GetSessionByIdAsync([Path] string id);
    [Get("session/gm/{gameMasterId}")]
    Task<IEnumerable<SessionDto>> GetSessionsByGameMasterIdAsync(
        [Path] string gameMasterId,
        [Query] int skip = 0,
        [Query] int limit = 10);
    [Get("session/characters/{characterId}")]
    Task<IEnumerable<SessionDto>> GetSessionsByCharacterIdAsync(
        [Path] string characterId,
        [Query] int skip = 0,
        [Query] int limit = 10);
    [Delete("session/{id}/players/{playerId}")]
    Task RemovePlayerFromSessionAsync([Path] string id, [Path] string playerId);
    [Delete("session/{id}/characters/{characterId}")]
    Task RemoveCharacterFromSessionAsync([Path] string id, [Path] string characterId);
    [Post("/session")] Task<SessionDto> CreateSessionAsync([Body] CreateSessionDto session);
    [Put("/session/{id}")] Task<SessionDto> UpdateSessionAsync([Path] string id, [Body] UpdateSessionDto session);
    [Delete("/session/{id}")] Task DeleteSessionAsync([Path] string id);
}