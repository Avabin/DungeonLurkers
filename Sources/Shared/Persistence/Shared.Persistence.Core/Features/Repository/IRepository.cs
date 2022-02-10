using System.Linq.Expressions;
using Shared.Features;
using Shared.MessageBroker.Core;
using Shared.Persistence.Core.Features.Documents;

namespace Shared.Persistence.Core.Features.Repository;

public interface IRepository<T, TId> where T : class, IDocument<TId>
{
    IObservable<DocumentChanged<T, TId>> DocumentChangedObservable { get; }
    Task<TField?> GetFieldAsync<TField>(Expression<Func<T, bool>> predicate, Expression<Func<T, TField>> field);
    Task<TId> InsertAsync(T doc);
    Task<IEnumerable<string>> InsertAsync(IEnumerable<T> docs);
    Task UpdateAsync(TId id, T doc);
    Task UpdateAllAsync<TField>(Expression<Func<T, bool>> predicate, Expression<Func<T, TField>> field, TField value);

    Task UpdateSingleAsync<TField>(
        Expression<Func<T, bool>>   predicate,
        Expression<Func<T, TField>> field,
        TField                      value);

    Task     DeleteAsync(TId                                   id);
    Task<T?> GetByIdAsync(TId                                  id);
    Task<T?> GetByFieldAsync<TProp>(Expression<Func<T, TProp>> propertyAccessor, TProp value);

    Task<IReadOnlyCollection<T>> GetAllByFieldAsync<TField>(
        Expression<Func<T, TField>> field,
        TField                      value,
        int?                        skip  = null,
        int?                        limit = null);

    Task<IReadOnlyCollection<T>> GetAllByPredicateAsync(
        Expression<Func<T, bool>> predicate,
        int?                      skip  = null,
        int?                      limit = null);

    Task<T?>                     GetByPredicateAsync(Expression<Func<T, bool>> predicate);
    Task<IReadOnlyCollection<T>> GetAllAsync(int?                              skip = null, int? limit = null);

    Task AddElementToArrayFieldAsync<TElement>(string   id, Expression<Func<T, IEnumerable<TElement>>> field,
                                                    TElement newElement);
    Task RemoveElementFromArrayFieldAsync<TElement>(string id, Expression<Func<T, IEnumerable<TElement>>> field, TElement like);
}