using Shared.Persistence.Core.Features.Documents.Many;
using TheDungeonGuide.Shared.Features.Characters;

namespace TheDungeonGuide.Persistence.Characters;

public interface ICharacterFacade : IDocumentFacade<CharacterDocument, string, CharacterDto>
{
    Task<IEnumerable<CharacterDto>> FindAllByOwnerIdAsync(string ownerId, int? skip = null, int? limit = null);
    
    Task<CharacterDto?> FindByNameAsync(string name);
}