using AutoMapper;
using Shared.Persistence.Core.Features.Documents.Many;
using Shared.Persistence.Core.Features.Repository;
using TheDungeonGuide.Shared.Features.Characters;

namespace TheDungeonGuide.Persistence.Characters.Features.Many;

public class ManyCharactersService
    : ManyDocumentsService<CharacterDocument, string, CharacterDto>, IManyCharactersService
{
    public ManyCharactersService(IRepository<CharacterDocument, string> repository, IMapper mapper) : base(repository,
        mapper)
    {
    }
}