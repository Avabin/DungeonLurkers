using Shared.Features;

namespace TheDungeonGuide.Shared.Features.Characters;

public record CharacterDto : IDocumentDto<string>
{
    public string Name    { get; set; } = "";
    public string Id      { get; set; } = "";
    public string OwnerId { get; set; } = "";
}