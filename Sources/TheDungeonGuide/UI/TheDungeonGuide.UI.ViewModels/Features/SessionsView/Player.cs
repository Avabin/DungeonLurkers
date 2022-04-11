using Shared.Features.Users;
using TheDungeonGuide.Shared.Features.Sessions;

namespace TheDungeonGuide.UI.ViewModels.Features.SessionsView;

public record Player(string Id, string UserName, List<Character> Characters)
{
    public override string ToString() => $"{UserName} ({Characters.Count})";
}