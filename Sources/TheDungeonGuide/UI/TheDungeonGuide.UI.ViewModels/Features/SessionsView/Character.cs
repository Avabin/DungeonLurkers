namespace TheDungeonGuide.UI.ViewModels.Features.SessionsView;

public record Character(string Id, string Name, string OwnerName)
{
    public override string ToString() => $"{Name} ({OwnerName})";
}