using AutoMapper;
using Shared.Persistence.Core.Features.Documents.Single;
using Shared.Persistence.Core.Features.Repository;
using TheDungeonGuide.Shared.Features.Characters;

namespace TheDungeonGuide.Persistence.Characters.Features.Single;

public class SingleCharacterService
    : SingleDocumentService<CharacterDocument, string, CharacterDto>, ISingleCharacterService
{
    public SingleCharacterService(IRepository<CharacterDocument, string> repository, IMapper mapper) : base(repository,
        mapper)
    {
    }

    public async Task<CharacterDto?> FindByNameAsync(string name)
    {
        var result = await Repository.GetByFieldAsync(x => x.Name, name);
        
        return result == null ? null : Mapper.Map<CharacterDto>(result);
    }
}