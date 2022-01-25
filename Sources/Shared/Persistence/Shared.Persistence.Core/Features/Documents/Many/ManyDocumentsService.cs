using System.Linq.Expressions;
using AutoMapper;
using Shared.Features;
using Shared.Persistence.Core.Features.Repository;

namespace Shared.Persistence.Core.Features.Documents.Many;

public class ManyDocumentsService<TDocument, TId, TFindDocumentDto>
    : DocumentServiceBase<TDocument, TId>, IManyDocumentsService<TDocument, TId, TFindDocumentDto>
    where TFindDocumentDto : IDocumentDto<TId> where TDocument : IDocument<TId>
{
    protected ManyDocumentsService(IRepository<TDocument, TId> repository, IMapper mapper) : base(repository, mapper)
    {
    }

    public async Task<IEnumerable<TFindDocumentDto>> GetAllAsync(int? skip = null, int? limit = null)
    {
        var list = await Repository.GetAllAsync(skip, limit);

        var mapped = list.Select(doc => Mapper.Map<TFindDocumentDto>(doc));

        return mapped;
    }

    public async Task<IEnumerable<TFindDocumentDto>> GetAllByPredicateAsync(
        Expression<Func<TDocument, bool>> predicate,
        int?                              skip  = null,
        int?                              limit = null)
    {
        var result = await Repository.GetAllByPredicateAsync(predicate, skip, limit);

        return Mapper.Map<IEnumerable<TFindDocumentDto>>(result);
    }

    public async Task<IEnumerable<TFindDocumentDto>> GetAllByFieldAsync<TField>(
        Expression<Func<TDocument, TField>> field,
        TField                              value,
        int?                                skip,
        int?                                limit)
    {
        var result = await Repository.GetAllByFieldAsync(field, value, skip, limit);

        return Mapper.Map<IEnumerable<TFindDocumentDto>>(result);
    }
}