using Shared.Features;

namespace TheDungeonGuide.Shared.Features.Characters;

public record CreateCharacterDto : ICreateDocumentDto
{
    public string OwnerId { get; set; } = "";
    public string Name { get; set; } = "";
}