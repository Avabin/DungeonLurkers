using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;
using Shared.Features;
using Shared.Features.Authentication;

namespace TheDungeonGuide.Shared.Features.Characters;

public interface ICharactersApi : IAuthenticatedApi
{
    [Get("character")]
    Task<IEnumerable<CharacterDto>> GetAllAsync([Query] int? skip = null, [Query] int? limit = null);

    [Get("character/{id}")] Task<CharacterDto> FindByIdAsync([Path] string id);

    [Get("character/name/{name}")] Task<CharacterDto> FindCharacterByNameAsync([Path] string name);

    [Get("character/owner/{ownerId}")] Task<IEnumerable<CharacterDto>> FindByOwnerIdAsync([Path] string ownerId, [Query] int? skip = null, [Query] int? limit = null);

    [Post("character")] Task<CharacterDto> CreateCharacterAsync([Body] CreateCharacterDto character);

    [Put("character/{id}")]
    Task UpdateCharacterAsync([Path] string id, [Body] UpdateCharacterDto character);

    [Delete("character/{id}")] Task DeleteCharacterAsync([Path] string id);
}