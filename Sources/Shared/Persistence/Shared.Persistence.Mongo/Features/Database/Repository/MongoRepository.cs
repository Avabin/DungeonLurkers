using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Extensions.Configuration;
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
    protected virtual   string       DatabaseName { get; }
    private readonly IMongoClient _client;

    private readonly string                      _collection = MongoDbHelper.GetCollectionName<T>();
    private readonly ILogger<MongoRepository<T>> _logger;

    public MongoRepository(IMongoClient client, ILogger<MongoRepository<T>> logger, IConfiguration  configuration)
    {
        _client = client;
        _logger = logger;
        
        var connectionString = configuration.GetConnectionString("MongoDb");
        
        var url = new MongoUrl(connectionString);

        if (url is { DatabaseName: not null or "" } settings)
        {
            _logger.LogDebug("Using custom database name: {DatabaseName}", settings.DatabaseName);
            DatabaseName = url.DatabaseName;
        }
        else
        {
            _logger.LogWarning("Using default database name: {DatabaseName}", MongoDbHelper.DatabaseName);
            DatabaseName = MongoDbHelper.DatabaseName;
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
        Expression<Func<T, TField>> field)
    {
        _logger.LogTrace("{Action} of {DocumentType} with predicate {Predicate} on field {Field}}",
                         nameof(GetFieldAsync), typeof(T).Name, predicate, field);
        return await Collection
                    .Find(predicate)
                    .Project(field)
                    .SingleOrDefaultAsync();
    }

    public async Task<string> InsertAsync(T doc)
    {
        _logger.LogTrace("{Action}: Doc Id = {DocumentId} of type {DocumentType}", nameof(InsertAsync), doc.Id,
                         typeof(T).Name);
        await Collection.InsertOneAsync(doc);
        _logger.LogTrace("{Action}: Publishing insert change event", nameof(InsertAsync));
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
        _logger.LogTrace("{Action}: Publishing insert (many) change event", nameof(InsertAsync));
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
        var oldDoc = await Collection.FindOneAndReplaceAsync(filter, castedDoc);
        var result = await Collection.Find(filter).SingleAsync();
        
        if (result is not null)
        {
            _logger.LogTrace("{Action}: Publishing update change event", nameof(UpdateAsync));
            _docChangedSubject.OnNext(DocumentChanged.Updated(oldDoc, result));
        }
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
        _logger.LogTrace("{Action} of {DocumentType} with predicate {Predicate} on field {Field} with value {@Value}",
                         nameof(UpdateAllAsync), typeof(T).Name, predicate, field, value);
        var update = Builders<T>.Update.Set(field, value);
        var oldDocs = await Collection.Find(predicate).ToListAsync();

        var updateResult = await Collection.UpdateManyAsync(predicate, update);
        var updated      = await Collection.Find(predicate).ToListAsync();

        if (updateResult.IsAcknowledged && updateResult.ModifiedCount > 0)
        {
            _logger.LogTrace("{Action}: Publishing update (many) change event", nameof(UpdateAllAsync));
            _docChangedSubject.OnNext(DocumentsChanged.Updated(oldDocs.Take((int)updateResult.ModifiedCount), updated.Take((int) updateResult.ModifiedCount)));
        }
        
        _logger.LogTrace("{Action}: Update result {@UpdateResult} docs of type {DocumentType}", updateResult, typeof(T).Name);
    }

    public async Task UpdateSingleAsync<TField>(
        Expression<Func<T, bool>>   predicate,
        Expression<Func<T, TField>> field,
        TField                      value)
    {
        _logger.LogTrace("{Action} of {DocumentType} with predicate {Predicate} on field {Field} with value {@Value}",
                         nameof(UpdateSingleAsync), typeof(T).Name, predicate, field, value);
        var update = Builders<T>.Update.Set(field, value);
        
        var oldDoc = await Collection.FindOneAndUpdateAsync(predicate, update);
        var updated = await Collection.Find(predicate).SingleAsync();

        if (updated is not null)
        {
            _logger.LogTrace("{Action}: Publishing update change event", nameof(UpdateSingleAsync));
            _docChangedSubject.OnNext(DocumentChanged.Updated(oldDoc, updated));
        }
        
        _logger.LogTrace("{Action}: Update result {@UpdateResult} field of doc of type {DocumentType}", updated, typeof(T).Name);
    }

    public async Task DeleteAsync(string id)
    {
        _logger.LogTrace("{Action}: Doc Id = {DocumentId} of type {DocumentType}", nameof(DeleteAsync), id,
                         typeof(T).Name);
        await ThrowIfDocWithIdNotFound(id);
        var oldDoc = await Collection.Find(x => x.Id == id).SingleAsync();
        var result = await Collection.DeleteOneAsync(f => Equals(f.Id, id));

        if (result.IsAcknowledged && result.DeletedCount > 0)
        {
            _logger.LogTrace("{Action}: Publishing delete change event", nameof(DeleteAsync));
            _docChangedSubject.OnNext(DocumentChanged.Deleted(oldDoc));
        }
        
        _logger.LogTrace("{Action}: Delete result {@DeleteResult} doc of type {DocumentType}", nameof(DeleteAsync),result, typeof(T).Name);
    }

    public async Task<T?> GetByIdAsync(string id)
    {
        _logger.LogTrace("{Action}: Doc Id = {DocumentId} of type {DocumentType}", nameof(GetByIdAsync), id,
                         typeof(T).Name);
        var filter = Builders<T>.Filter.Eq(s => s.Id, id);
        return await Collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<T?> GetByFieldAsync<TProp>(Expression<Func<T, TProp>> field, TProp value)
    {
        _logger.LogTrace("{Action} of {DocumentType} on field {Field} with value {@Value}",
                         nameof(GetByFieldAsync), typeof(T).Name, field, value);
        var filter = Builders<T>.Filter.Eq(field, value);
        return await Collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyCollection<T>> GetAllByFieldAsync<TProp>(
        Expression<Func<T, TProp>> field,
        TProp                      value,
        int?                       skip  = null,
        int?                       limit = null)
    {
        _logger.LogTrace("{Action} of {DocumentType} ({Skip}, {Limit}) on field {Field} with value {@Value}",
                         nameof(GetAllByFieldAsync), typeof(T).Name, skip, limit, field, value);
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
        _logger.LogTrace("{Action} of {DocumentType} ({Skip}, {Limit}) with predicate {Predicate}",
                         nameof(GetAllByPredicateAsync), typeof(T).Name, skip, limit, predicate);
        var filter = Builders<T>.Filter.Where(predicate);
        return await Collection
                    .Find(filter)
                    .Skip(skip)
                    .Limit(limit)
                    .ToListAsync();
    }

    public async Task<T?> GetByPredicateAsync(Expression<Func<T, bool>> predicate)
    {
        _logger.LogTrace("{Action} of {DocumentType} with predicate {Predicate}", nameof(GetByPredicateAsync),
                           typeof(T).Name, predicate);

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
        var result = await Collection.FindOneAndUpdateAsync(filter, update);
        if(result is null) throw new DocumentNotFoundException($"Document with id {id} not found");
        var newDoc = await Collection.Find(filter).SingleOrDefaultAsync();
        
        _logger.LogTrace("{Action}: Result of update: {@Result}", nameof(AddElementToArrayFieldAsync), newDoc);

        if (newDoc is not null)
        {
            _logger.LogTrace("{Action}: Publishing update change event", nameof(AddElementToArrayFieldAsync));
            _docChangedSubject.OnNext(DocumentChanged.Updated(result, newDoc));
        }
    }
    
    public async Task RemoveElementFromArrayFieldAsync<TElement>(string id, Expression<Func<T, IEnumerable<TElement>>> field, TElement like)
    {
        _logger.LogTrace("{Action}: Removing element {Element} from field {Field} of type {DocumentType}",
                         nameof(RemoveElementFromArrayFieldAsync), like, field, typeof(T).Name);
        
        var filter = Builders<T>.Filter.Eq(s => s.Id, id);
        var update = Builders<T>.Update.Pull(field, like);
        var result = await Collection.FindOneAndUpdateAsync(filter, update);
        if(result is null) throw new DocumentNotFoundException($"Document with id {id} not found");
        var newDoc = await Collection.Find(filter).SingleOrDefaultAsync();
        
        _logger.LogTrace("{Action}: Result of update: {@Result}", nameof(RemoveElementFromArrayFieldAsync), newDoc);

        if (newDoc is not null)
        {
            _logger.LogTrace("{Action}: Publishing update change event", nameof(RemoveElementFromArrayFieldAsync));
            _docChangedSubject.OnNext(DocumentChanged.Updated(result, newDoc));
        }
    }

    public async Task<IEnumerable<TElement>> GetArrayFieldAsync<TElement>(string id, Expression<Func<T, IEnumerable<TElement>>> field)
    {
        _logger.LogTrace("{Action} of {DocumentType} on field {Field}", nameof(GetArrayFieldAsync), typeof(T).Name, field);
        
        var            filter = Builders<T>.Filter.Eq(s => s.Id, id);
        var result = await Collection.Find(filter).Project(field).SingleOrDefaultAsync();

        if(result is null) throw new DocumentNotFoundException($"Document with id {id} not found");
        
        return result;
    }

    public async Task<IEnumerable<TField>> GetFieldsAsync<TField>(
        Expression<Func<T, TField>> field,
        int?                        skip  = null,
        int?                        limit = null)
    {
        _logger.LogTrace("{Action} of {DocumentType} ({Skip}, {Limit}) on field {Field}", nameof(GetFieldsAsync), typeof(T).Name, skip,
                         limit, field);
        
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
        _logger.LogTrace("{Action} of {DocumentType} ({Skip}, {Limit}) with predicate {Predicate} on field {Field}",
                         nameof(GetFieldsByPredicateAsync), typeof(T).Name, skip, limit, predicate, field);
        
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
        if (result.IsAcknowledged && result.DeletedCount > 0)
        {
            _logger.LogTrace("{Action}: Publishing delete (many) change event", nameof(DeleteManyAsync));
            _docChangedSubject.OnNext(DocumentsChanged.Deleted(oldDocs.Take((int)result.DeletedCount)));
        }
        
        _logger.LogTrace("{Action} of {DocumentType} ({Ids}). Modified {Count}", nameof(DeleteManyAsync), typeof(T).Name,
                           string.Join(", ", idsList), result.DeletedCount);
    }
}