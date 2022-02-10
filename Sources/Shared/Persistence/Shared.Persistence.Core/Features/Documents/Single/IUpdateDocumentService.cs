using System.Linq.Expressions;
using Shared.Features;

namespace Shared.Persistence.Core.Features.Documents.Single;

public interface IUpdateDocumentService<TDocument, in TId> where TDocument : IDocument<TId>
{
    Task UpdateAsync<TField>(
        Expression<Func<TDocument, bool>>   predicate,
        Expression<Func<TDocument, TField>> field,
        TField                              value);

    Task UpdateAsync<TUpdateDto>(TId id, TUpdateDto dto);
}