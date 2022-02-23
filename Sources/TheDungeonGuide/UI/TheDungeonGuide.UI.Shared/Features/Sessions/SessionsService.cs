using System.Net;
using Microsoft.Extensions.Logging;
using RestEase;
using TheDungeonGuide.Shared.Features.Sessions;

namespace TheDungeonGuide.UI.Shared.Features.Sessions;

internal class SessionsService : ISessionsService
{
    private readonly ISessionsApi             _api;
    private readonly ILogger<SessionsService> _logger;
    public SessionsService(ISessionsApi api, ILogger<SessionsService> logger)
    {
        _api    = api;
        _logger = logger;
        
    }
    
    public async Task<IEnumerable<SessionDto>> GetSessionsByUserId(string userId)
    {
        _logger.LogInformation("Getting sessions for user {UserId}", userId);

        try
        {
            var sessions = await _api.GetSessionsByUserIdAsync(userId);
            return sessions;
        }
        catch (ApiException e) when (e.StatusCode == HttpStatusCode.NotFound)
        {
            return Enumerable.Empty<SessionDto>();
        }
    }
}