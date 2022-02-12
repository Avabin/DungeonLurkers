using Shared.Persistence.Core.Features.Documents.Many;
using TheDungeonGuide.Persistence.Characters.Features.Many;
using TheDungeonGuide.Persistence.Characters.Features.Single;
using TheDungeonGuide.Shared.Features.Characters;

namespace TheDungeonGuide.Persistence.Characters;

internal class CharacterFacade : DocumentFacade<CharacterDocument, string, CharacterDto>, ICharacterFacade
{
    private readonly ISingleCharacterService _singleDocumentService;

    public CharacterFacade(
        ISingleCharacterService singleDocumentService,
        IManyCharactersService  manyDocumentsService) :
        base(singleDocumentService, manyDocumentsService)
    {
        _singleDocumentService = singleDocumentService;
    }
    public Task<IEnumerable<CharacterDto>> FindAllByOwnerIdAsync(
        string ownerId,
        int?   skip  = null,
        int?   limit = null) =>
        ManyDocumentsService.GetAllByPredicateAsync(x => x.OwnerId == ownerId, skip, limit);

    public async Task<CharacterDto?> FindByNameAsync(string name) => 
        await _singleDocumentService.FindByNameAsync(name);
}