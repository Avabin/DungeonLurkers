using System.Linq.Expressions;
using AutoMapper;

namespace Shared.Persistence.Identity.Features;

public abstract class ManyDocumentServiceBase<TDocument, TDto>
{
    private readonly IMapper _mapper;

    protected ManyDocumentServiceBase(IMapper mapper) => _mapper = mapper;

    protected IEnumerable<TDto> GetAll(int? skip, int? limit, IQueryable<TDocument> queryable)
    {
        var docs                   = queryable;
        if (skip is { } i) docs    = docs.Skip(i);
        if (limit is { } max) docs = docs.Take(max);

        return docs.AsEnumerable().Select(x => _mapper.Map<TDto>(x));
    }

    protected IEnumerable<TDto> GetAllByPredicate(
        Expression<Func<TDocument, bool>> predicate,
        int?                              skip,
        int?                              limit,
        IQueryable<TDocument>             queryable)
    {
        var docs                   = queryable.Where(predicate);
        if (skip is { } i) docs    = docs.Skip(i);
        if (limit is { } max) docs = docs.Take(max);

        return docs.AsEnumerable().Select(x => _mapper.Map<TDto>(x));
    }
}