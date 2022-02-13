using System.Linq.Expressions;
using Shared.Features;
using Shared.Features.Roles;

namespace Shared.Persistence.Core.Features.Documents.Single;

public interface ISingleDocumentService<TDocument, in TId, TFindDocumentDto>
    : IUpdateDocumentService<TDocument, TId>,
      IRemoveDocumentService<TDocument, TId, TFindDocumentDto>
    where TDocument : IDocument<TId>
    where TFindDocumentDto : IDocumentDto<TId>
{
    Task<TFindDocumentDto> CreateAsync<TCreateDocDto>(TCreateDocDto request);

    Task<TField?> GetFieldAsync<TField>(
        Expression<Func<TDocument, bool>>   predicate,
        Expression<Func<TDocument, TField>> field);

    Task<TFindDocumentDto?> GetByIdAsync(TId id);

    Task<TFindDocumentDto?> GetByFieldAsync<TField>(
        Expression<Func<TDocument, TField>> field,
        TField                              value);

    Task<TFindDocumentDto?> GetByPredicateAsync(Expression<Func<TDocument, bool>> predicate);
}