using System.Linq.Expressions;
using Shared.Features;
using Shared.Persistence.Core.Features.Documents;
using Shared.Persistence.Core.Features.Repository;

namespace Shared.Persistence.Mongo.Features.Database.Repository;

public interface IMongoRepository<T> : IRepository<T, string> where T : class, IDocument<string>
{
    Task<IEnumerable<TField>> GetFieldsAsync<TField>(
        Expression<Func<T, TField>> field,
        int?                        skip  = null,
        int?                        limit = null);
    Task<IEnumerable<TField>> GetFieldsByPredicateAsync<TField>(
        Expression<Func<T, bool>>   predicate,
        Expression<Func<T, TField>> field,
        int?                        skip  = null,
        int?                        limit = null);
    Task DeleteManyAsync(IEnumerable<string> ids);
}