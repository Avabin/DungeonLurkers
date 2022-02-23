using TheDungeonGuide.Shared.Features.Sessions;

namespace TheDungeonGuide.UI.ViewModels.Features.SessionsView;

public record Session : SessionDto
{
    public           bool   IsCurrentUserGm => GameMasterId == _currentUserId;

    public int PlayersCount => PlayersIds.Count() + 1;
    
    public int CharactersCount => CharactersIds.Count();
    
    private readonly string? _currentUserId;

    public Session(SessionDto sessionDto, string currentUserId = "") : base(sessionDto)
    {
        _currentUserId = currentUserId;
    }
    
    public static Session Of(SessionDto sessionDto, string currentUserId = "") => new(sessionDto, currentUserId);
}