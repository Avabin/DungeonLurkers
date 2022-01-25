using System.Linq.Expressions;
using Shared.Features;

namespace Shared.Persistence.Core.Features.Documents.Many;

public interface IManyDocumentsService<TDocument, in TId, TFindDocumentDto>
    where TDocument : IDocument<TId> where TFindDocumentDto : IDocumentDto<TId>
{
    Task<IEnumerable<TFindDocumentDto>> GetAllAsync(int? skip = null, int? limit = null);
    Task<IEnumerable<TFindDocumentDto>> GetAllByPredicateAsync(
        Expression<Func<TDocument, bool>> predicate,
        int?                              skip = null,
        int?                              limit = null);
}