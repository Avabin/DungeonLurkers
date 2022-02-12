using Shared.Persistence.Core.Features.Documents.Many;
using TheDungeonGuide.Persistence.Characters.Features.Many;
using TheDungeonGuide.Persistence.Characters.Features.Single;
using TheDungeonGuide.Shared.Features.Characters;

namespace TheDungeonGuide.Persistence.Characters;

internal class CharacterFacade : DocumentOperationFacade<CharacterDocument, string, CharacterDto>, ICharacterFacade
{
    private readonly ISingleCharacterService _singleSingleDocumentService;

    public CharacterFacade(
        ISingleCharacterService singleSingleDocumentService,
        IManyCharactersService  manyManyDocumentsService) :
        base(singleSingleDocumentService, manyManyDocumentsService)
    {
        _singleSingleDocumentService = singleSingleDocumentService;
    }
    public Task<IEnumerable<CharacterDto>> FindAllByOwnerIdAsync(
        string ownerId,
        int?   skip  = null,
        int?   limit = null) =>
        ManyDocumentsService.GetAllByPredicateAsync(x => x.OwnerId == ownerId, skip, limit);

    public async Task<CharacterDto?> FindByNameAsync(string name) => 
        await _singleSingleDocumentService.FindByNameAsync(name);
}