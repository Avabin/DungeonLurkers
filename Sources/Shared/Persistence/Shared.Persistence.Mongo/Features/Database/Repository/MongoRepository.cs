using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Shared.Features;
using Shared.MessageBroker.Core;
using Shared.MessageBroker.Persistence;
using Shared.Persistence.Core.Features.Documents;
using Shared.Persistence.Core.Features.Exceptions;
using Shared.Persistence.Mongo.Features.Database.Documents;

#pragma warning disable CS8619

#pragma warning disable 8604

namespace Shared.Persistence.Mongo.Features.Database.Repository;

internal class MongoRepository<T> : IMongoRepository<T> where T : class, IDocument<string>
{
    public virtual   string       DatabaseName { get; private set; } = MongoDbHelper.DatabaseName;
    private readonly IMongoClient _client;

    private readonly string                      _collection = MongoDbHelper.GetCollectionName<T>();
    private readonly ILogger<MongoRepository<T>> _logger;

    public MongoRepository(IMongoClient client, ILogger<MongoRepository<T>> logger, IOptions<MongoSettings> options)
    {
        _client = client;
        _logger = logger;

        if (options.Value is { DatabaseName: not null or "" } settings)
        {
            _logger.LogDebug("Using custom database name: {DatabaseName}", settings.DatabaseName);
            DatabaseName = options.Value.DatabaseName;
        }
        else
        {
            _logger.LogWarning("Using default database name: {DatabaseName}", MongoDbHelper.DatabaseName);
        }

        if (!_client.GetDatabase(DatabaseName).ListCollectionNames().ToList().Contains(_collection))
            _client.GetDatabase(DatabaseName).CreateCollection(_collection);
    }

    protected IMongoCollection<T> Collection =>
        _client.GetDatabase(DatabaseName).GetCollection<T>(_collection);

    private readonly ISubject<DocumentChangeBase<T, string>>    _docChangedSubject = new Subject<DocumentChangeBase<T, string>>();
    public           IObservable<DocumentChanged<T, string>> DocumentChangedObservable => _docChangedSubject.OfType<DocumentChanged<T, string>>();
    public           IObservable<DocumentsChanged<T, string>> DocumentsChangedObservable => _docChangedSubject.OfType<DocumentsChanged<T, string>>();

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
        _docChangedSubject.OnNext(DocumentChanged.Inserted(doc));
        return doc.Id;
    }

    public async Task<IEnumerable<string>> InsertAsync(IEnumerable<T> docs)
    {
        var docsList = docs.ToList();
        var docsIds  = docsList.Select(x => x.Id).ToList();
        _logger.LogTrace("{Action}: Docs ids = [{DocsIds}] of type {DocumentType}", nameof(InsertAsync),
                         string.Join(",", docsIds), typeof(T).Name);

        await Collection.InsertManyAsync(docsList);
        _docChangedSubject.OnNext(DocumentsChanged.Inserted(docsList));

        _logger.LogTrace("Inserted {Count} docs of type {DocumentType}", docsList.Count, typeof(T).Name);
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
        _docChangedSubject.OnNext(DocumentChanged.Updated(doc));
        _logger.LogTrace("Updated doc Id = {DocumentId} of type {DocumentType}", doc.Id, typeof(T).Name);
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
        var oldDocs = await Collection.Find(predicate).ToListAsync();

        var updateResult = await Collection.UpdateManyAsync(predicate, update);
        
        _docChangedSubject.OnNext(DocumentsChanged.Updated(oldDocs));
        
        _logger.LogTrace("{Action}: Update result {@UpdateResult} docs of type {DocumentType}", updateResult, typeof(T).Name);
    }

    public async Task UpdateSingleAsync<TField>(
        Expression<Func<T, bool>>   predicate,
        Expression<Func<T, TField>> field,
        TField                      value)
    {
        var update = Builders<T>.Update.Set(field, value);

        var oldDoc = await Collection.Find(predicate).SingleAsync();

        var result = await Collection.UpdateOneAsync(predicate, update);
        
        if (result.ModifiedCount > 0) _docChangedSubject.OnNext(DocumentChanged.Updated(oldDoc));
        
        _logger.LogTrace("{Action}: Update result {@UpdateResult} field of doc of type {DocumentType}", result, typeof(T).Name);
    }

    public async Task DeleteAsync(string id)
    {
        _logger.LogTrace("{Action}: Doc Id = {DocumentId} of type {DocumentType}", nameof(InsertAsync), id,
                         typeof(T).Name);
        await ThrowIfDocWithIdNotFound(id);
        var oldDoc = await Collection.Find(x => x.Id == id).SingleAsync();
        var result = await Collection.DeleteOneAsync(f => Equals(f.Id, id));
        if (result.DeletedCount > 0) _docChangedSubject.OnNext(DocumentChanged.Deleted(oldDoc));
        
        _logger.LogTrace("{Action}: Delete result {@DeleteResult} doc of type {DocumentType}", result, typeof(T).Name);
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
        var result = await Collection
                             .Find(FilterDefinition<T>.Empty)
                             .Skip(skip)
                             .Limit(limit)
                             .ToListAsync();
        
        _logger.LogTrace("Found {Count} documents of type {DocumentType}", result.Count, typeof(T).Name);

        return result;
    }

    public async Task AddElementToArrayFieldAsync<TElement>(string id, Expression<Func<T, IEnumerable<TElement>>> field, TElement newElement)
    {
        _logger.LogTrace("{Action}: Adding element {Element} to field {Field} of type {DocumentType}",
                         nameof(AddElementToArrayFieldAsync), newElement, field, typeof(T).Name);
        
        var filter = Builders<T>.Filter.Eq(s => s.Id, id);
        var update = Builders<T>.Update.AddToSet(field, newElement);
        var oldDoc = await Collection.Find(filter).SingleAsync();
        if(oldDoc is null) throw new DocumentNotFoundException($"Document with id {id} not found");
        var result = await Collection.UpdateOneAsync(filter, update);
        
        _logger.LogTrace("{Action}: Result of update: {@Result}", nameof(AddElementToArrayFieldAsync), result);

        if (result.IsAcknowledged && result.ModifiedCount > 0)
            _docChangedSubject.OnNext(DocumentChanged.Updated(oldDoc));
    }
    
    public async Task RemoveElementFromArrayFieldAsync<TElement>(string id, Expression<Func<T, IEnumerable<TElement>>> field, TElement like)
    {
        _logger.LogTrace("{Action}: Removing element {Element} from field {Field} of type {DocumentType}",
                         nameof(RemoveElementFromArrayFieldAsync), like, field, typeof(T).Name);
        
        var filter = Builders<T>.Filter.Eq(s => s.Id, id);
        var update = Builders<T>.Update.Pull(field, like);
        var oldDoc = await Collection.Find(filter).SingleAsync();
        if(oldDoc is null) throw new DocumentNotFoundException($"Document with id {id} not found");
        var result = await Collection.UpdateOneAsync(filter, update);
        
        _logger.LogTrace("{Action}: Result of update: {@Result}", nameof(RemoveElementFromArrayFieldAsync), result);

        if (result.IsAcknowledged && result.ModifiedCount > 0)
            _docChangedSubject.OnNext(DocumentChanged.Updated(oldDoc));
    }

    public async Task<IEnumerable<TField>> GetFieldsAsync<TField>(
        Expression<Func<T, TField>> field,
        int?                        skip  = null,
        int?                        limit = null)
    {
        _logger.LogTrace("{Action} of {DocumentType} ({Skip}, {Limit})", nameof(GetAllAsync), typeof(T).Name, skip,
                           limit);
        var result =await Collection
                    .Find(FilterDefinition<T>.Empty)
                    .Project(field)
                    .Skip(skip)
                    .Limit(limit)
                    .ToListAsync();
        
        _logger.LogTrace("Found {Count} fields of documents of type {DocumentType}", result.Count, typeof(T).Name);
        return result;
    }

    public async Task<IEnumerable<TField>> GetFieldsByPredicateAsync<TField>(
        Expression<Func<T, bool>>   predicate,
        Expression<Func<T, TField>> field,
        int?                        skip  = null,
        int?                        limit = null)
    {
        _logger.LogTrace("{Action} of {DocumentType} ({Skip}, {Limit})", nameof(GetAllAsync), typeof(T).Name, skip,
                           limit);
        var result = await Collection
                    .Find(predicate)
                    .Project(field)
                    .Skip(skip)
                    .Limit(limit)
                    .ToListAsync();
        
        _logger.LogTrace("{Action}: Found {Count} fields of documents of type {DocumentType}", nameof(GetFieldsByPredicateAsync), result.Count, typeof(T).Name);
        return result;
    }

    public async Task DeleteManyAsync(IEnumerable<string> ids)
    {
        var idsList = ids.ToList();
        _logger.LogTrace("{Action} of {DocumentType} ({Ids})", nameof(DeleteManyAsync), typeof(T).Name,
                           string.Join(", ", idsList));
        var oldDocs = await Collection.Find(Builders<T>.Filter.In(x => x.Id, idsList)).ToListAsync();
        var result = await Collection.DeleteManyAsync(Builders<T>.Filter.In(x => x.Id, idsList));
        _docChangedSubject.OnNext(DocumentsChanged.Deleted(oldDocs));
        
        _logger.LogTrace("{Action} of {DocumentType} ({Ids}). Modified {Count}", nameof(DeleteManyAsync), typeof(T).Name,
                           string.Join(", ", idsList), result.DeletedCount);
    }
}