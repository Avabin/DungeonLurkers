using Shared.Features;
using TheDungeonGuide.Shared.Features.Characters;

namespace TheDungeonGuide.Persistence.Characters;

public class PersistenceCharactersMapperProfile
    : DtoMapperProfile<CharacterDto, CreateCharacterDto, UpdateCharacterDto, CharacterDocument>
{
}