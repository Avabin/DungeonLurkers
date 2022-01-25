using Shared.Features;

namespace TheDungeonGuide.Shared.Features.Characters;

public record CreateCharacterDto : ICreateDocumentDto
{
    public string Name { get; set; } = "";
}