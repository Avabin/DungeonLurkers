using Shared.Persistence.Core.Features.Documents.Many;
using TheDungeonGuide.Shared.Features.Characters;

namespace TheDungeonGuide.Persistence.Characters.Features.Many;

public interface IManyCharactersService : IManyDocumentsService<CharacterDocument, string, CharacterDto>
{
}