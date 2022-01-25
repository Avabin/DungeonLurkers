using Shared.Persistence.Core.Features.Documents.Single;
using TheDungeonGuide.Shared.Features.Characters;

namespace TheDungeonGuide.Persistence.Characters.Features.Single;

public interface ISingleCharacterService : ISingleDocumentService<CharacterDocument, string, CharacterDto>
{
    Task<CharacterDto?> FindByNameAsync(string name);
}