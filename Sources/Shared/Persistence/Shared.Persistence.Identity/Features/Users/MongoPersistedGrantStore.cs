using IdentityServer4.Models;
using IdentityServer4.Stores;
using Shared.Persistence.Mongo.Features.Database.Repository;

namespace Shared.Persistence.Identity.Features.Users;

public class MongoPersistedGrantStore : IPersistedGrantStore
{
    private readonly IMongoRepository<MongoPersistedGrantDocument> _repository;

    public MongoPersistedGrantStore(IMongoRepository<MongoPersistedGrantDocument> repository) =>
        _repository = repository;

    public Task StoreAsync(PersistedGrant grant) => _repository.InsertAsync(new MongoPersistedGrantDocument(grant));

    public async Task<PersistedGrant> GetAsync(string key)
    {
        var maybeGrant = await _repository.GetByFieldAsync(x => x.Key, key);
        if (maybeGrant is null) throw new ArgumentException($"Grant with key {key} not found");
        return maybeGrant;
    }

    public async Task<IEnumerable<PersistedGrant>> GetAllAsync(PersistedGrantFilter filter)
    {
        return await _repository.GetAllByPredicateAsync(
                                                        x =>
                                                            x.SubjectId == filter.SubjectId
                                                         && x.ClientId  == filter.ClientId
                                                         && x.Type      == filter.Type
                                                         && x.SessionId == filter.SessionId);
    }

    public async Task RemoveAsync(string key)
    {
        var maybeId = await _repository.GetFieldAsync(x => x.Key == key, x => x.Id);
        if (maybeId is null) throw new ArgumentException($"Grant with key {key} not found");
        await _repository.DeleteAsync(maybeId);
    }

    public async Task RemoveAllAsync(PersistedGrantFilter filter)
    {
        var ids = await _repository.GetFieldsByPredicateAsync(x =>
                                                                  x.SubjectId == filter.SubjectId
                                                               && x.ClientId  == filter.ClientId
                                                               && x.Type      == filter.Type
                                                               && x.SessionId == filter.SessionId,
                                                              x => x.Id);

        await _repository.DeleteManyAsync(ids);
    }
}