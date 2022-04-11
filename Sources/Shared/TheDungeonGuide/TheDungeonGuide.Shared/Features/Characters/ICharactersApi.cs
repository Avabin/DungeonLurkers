using RestEase;
using Shared.Features.Authentication;

namespace TheDungeonGuide.Shared.Features.Characters;

public interface ICharactersApi : IAuthenticatedApi
{
    [Get("characters")]
    Task<IEnumerable<CharacterDto>> GetAllCharactersAsync([Query] int? skip = null, [Query] int? limit = null);

    [Get("characters/{id}")] Task<CharacterDto> FindCharacterByIdAsync([Path] string id);

    [Get("characters/name/{name}")] Task<CharacterDto> FindCharacterByNameAsync([Path] string name);

    [Get("characters/owner/{ownerId}")] Task<IEnumerable<CharacterDto>> FindCharactersByOwnerIdAsync([Path] string ownerId, [Query] int? skip = null, [Query] int? limit = null);

    [Post("characters")] Task<CharacterDto> CreateCharacterAsync([Body] CreateCharacterDto character);

    [Put("characters/{id}")]
    Task UpdateCharacterAsync([Path] string id, [Body] UpdateCharacterDto character);

    [Delete("characters/{id}")] Task DeleteCharacterAsync([Path] string id);
}