using Shared.Features;

namespace TheDungeonGuide.Shared.Features.Characters;

public record UpdateCharacterDto : IUpdateDocumentDto
{
    public string? Name { get; set; }
    public string?  OwnerId { get; set; }
}