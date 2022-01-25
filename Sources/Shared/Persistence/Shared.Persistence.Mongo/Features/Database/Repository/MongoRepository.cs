using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Shared.Persistence.Core.Features.Documents;
using Shared.Persistence.Core.Features.Exceptions;
using Shared.Persistence.Mongo.Features.Database.Documents;

#pragma warning disable CS8619

#pragma warning disable 8604

namespace Shared.Persistence.Mongo.Features.Database.Repository;

internal class MongoRepository<T> : IMongoRepository<T> where T : class, IDocument<string>
{
    protected string       Database = MongoDbHelper.DatabaseName;
    private readonly IMongoClient _client;

    private readonly string                      _collection = MongoDbHelper.GetCollectionName<T>();
    private readonly ILogger<MongoRepository<T>> _logger;

    public MongoRepository(IMongoClient client, ILogger<MongoRepository<T>> logger, IOptions<MongoSettings> options)
    {
        _client = client;
        _logger = logger;

        if (options.Value is { DatabaseName: not null or "" } settings)
        {
            _logger.LogInformation("Using custom database name: {DatabaseName}", settings.DatabaseName);
            Database = options.Value.DatabaseName;
        }
        else
        {
            _logger.LogWarning("Using default database name: {DatabaseName}", MongoDbHelper.DatabaseName);
        }

        if (!_client.GetDatabase(Database).ListCollectionNames().ToList().Contains(_collection))
            _client.GetDatabase(Database).CreateCollection(_collection);
    }

    protected IMongoCollection<T> Collection =>
        _client.GetDatabase(Database).GetCollection<T>(_collection);

    public async Task<TField?> GetFieldAsync<TField>(
        Expression<Func<T, bool>>   predicate,
        Expression<Func<T, TField>> field) =>
        await Collection
           .Find(predicate)
           .Project(field)
           .SingleOrDefaultAsync();

    public async Task<string> InsertAsync(T doc)
    {
        _logger.LogTrace("{Action}: Doc Id = {DocumentId} of type {DocumentType}", nameof(InsertAsync), doc.Id,
                         typeof(T).Name);
        await Collection.InsertOneAsync(doc);

        return doc.Id;
    }

    public async Task<IEnumerable<string>> InsertAsync(IEnumerable<T> docs)
    {
        var docsList = docs.ToList();
        var docsIds  = docsList.Select(x => x.Id).ToList();
        _logger.LogTrace("{Action}: Docs ids = [{DocsIds}] of type {DocumentType}", nameof(InsertAsync),
                         string.Join(",", docsIds), typeof(T).Name);

        await Collection.InsertManyAsync(docsList);

        return docsIds;
    }

    public async Task UpdateAsync(string id, T doc)
    {
        if (doc as DocumentBase<string> is not { } docBase) return;
        _logger.LogTrace("{Action}: Doc Id = {DocumentId} of type {DocumentType}", nameof(UpdateAllAsync), doc.Id,
                         typeof(T).Name);

        await ThrowIfDocWithIdNotFound(id);

        var updatedDoc = docBase with
        {
            Id = id,
        };

        if (updatedDoc as T is not { } castedDoc) return;
        var filter = Builders<T>.Filter.Eq(x => x.Id, id);

        await Collection.ReplaceOneAsync(filter, castedDoc);
    }

    private async Task ThrowIfDocWithIdNotFound(string id)
    {
        if (await Collection.Find(x => x.Id == id).CountDocumentsAsync() < 1)
            throw new DocumentNotFoundException($"Document with ID = {id} not found");
    }

    public async Task UpdateAllAsync<TField>(
        Expression<Func<T, bool>>   predicate,
        Expression<Func<T, TField>> field,
        TField                      value)
    {
        var update = Builders<T>.Update.Set(field, value);

        await Collection.UpdateManyAsync(predicate, update);
    }

    public async Task UpdateSingleAsync<TField>(
        Expression<Func<T, bool>>   predicate,
        Expression<Func<T, TField>> field,
        TField                      value)
    {
        var update = Builders<T>.Update.Set(field, value);

        await Collection.UpdateOneAsync(predicate, update);
    }

    public async Task DeleteAsync(string id)
    {
        _logger.LogTrace("{Action}: Doc Id = {DocumentId} of type {DocumentType}", nameof(InsertAsync), id,
                         typeof(T).Name);
        await ThrowIfDocWithIdNotFound(id);
        await Collection.DeleteOneAsync(f => Equals(f.Id, id));
    }

    public async Task<T?> GetByIdAsync(string id)
    {
        _logger.LogTrace("{Action}: Doc Id = {DocumentId} of type {DocumentType}", nameof(InsertAsync), id,
                         typeof(T).Name);
        var filter = Builders<T>.Filter.Eq(s => s.Id, id);
        return await Collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<T?> GetByFieldAsync<TProp>(Expression<Func<T, TProp>> propertyAccessor, TProp value)
    {
        _logger.LogTrace(
            "{Action}: Searching entity for property of type {PropertyType} and value {PropertyValue} of type {DocumentType}",
            nameof(GetByFieldAsync), typeof(T).Name, value, typeof(T).Name);
        var filter = Builders<T>.Filter.Eq(propertyAccessor, value);
        return await Collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyCollection<T>> GetAllByFieldAsync<TProp>(
        Expression<Func<T, TProp>> field,
        TProp                      value,
        int?                       skip  = null,
        int?                       limit = null)
    {
        _logger.LogTrace(
            "{Action}: Searching entity for property of type {PropertyType} and value {PropertyValue} of type {DocumentType}",
            nameof(GetByFieldAsync), typeof(T).Name, value, typeof(T).Name);
        var filter = Builders<T>.Filter.Eq(field, value);
        return await Collection
                    .Find(filter)
                    .Skip(skip)
                    .Limit(limit)
                    .ToListAsync();
    }

    public async Task<IReadOnlyCollection<T>> GetAllByPredicateAsync(
        Expression<Func<T, bool>> predicate,
        int?                      skip  = null,
        int?                      limit = null)
    {
        _logger.LogTrace("{Action} of {DocumentType} ({Skip}, {Limit})", nameof(GetAllByPredicateAsync),
                           typeof(T).Name, skip, limit);
        var filter = Builders<T>.Filter.Where(predicate);
        return await Collection
                    .Find(filter)
                    .Skip(skip)
                    .Limit(limit)
                    .ToListAsync();
    }

    public async Task<T?> GetByPredicateAsync(Expression<Func<T, bool>> predicate)
    {
        _logger.LogTrace("{Action} of {DocumentType}", nameof(GetAllByPredicateAsync), typeof(T).Name);

        var filter = Builders<T>.Filter.Where(predicate);

        return await Collection.Find(filter).SingleOrDefaultAsync();
    }

    public async Task<IReadOnlyCollection<T>> GetAllAsync(int? skip = null, int? limit = null)
    {
        _logger.LogTrace("{Action} of {DocumentType} ({Skip}, {Limit})", nameof(GetAllAsync), typeof(T).Name, skip,
                           limit);
        return await Collection
                    .Find(FilterDefinition<T>.Empty)
                    .Skip(skip)
                    .Limit(limit)
                    .ToListAsync();
    }

    public async Task<IEnumerable<TField>> GetFieldsAsync<TField>(
        Expression<Func<T, TField>> field,
        int?                        skip  = null,
        int?                        limit = null)
    {
        _logger.LogTrace("{Action} of {DocumentType} ({Skip}, {Limit})", nameof(GetAllAsync), typeof(T).Name, skip,
                           limit);
        return await Collection
                    .Find(FilterDefinition<T>.Empty)
                    .Project(field)
                    .Skip(skip)
                    .Limit(limit)
                    .ToListAsync();
    }

    public async Task<IEnumerable<TField>> GetFieldsByPredicateAsync<TField>(
        Expression<Func<T, bool>>   predicate,
        Expression<Func<T, TField>> field,
        int?                        skip  = null,
        int?                        limit = null)
    {
        _logger.LogTrace("{Action} of {DocumentType} ({Skip}, {Limit})", nameof(GetAllAsync), typeof(T).Name, skip,
                           limit);
        return await Collection
                    .Find(predicate)
                    .Project(field)
                    .Skip(skip)
                    .Limit(limit)
                    .ToListAsync();
    }

    public Task DeleteManyAsync(IEnumerable<string> ids)
    {
        var idsList = ids.ToList();
        _logger.LogTrace("{Action} of {DocumentType} ({Ids})", nameof(DeleteManyAsync), typeof(T).Name,
                           string.Join(", ", idsList));
        return Collection.DeleteManyAsync(Builders<T>.Filter.In(x => x.Id, idsList));
    }
}