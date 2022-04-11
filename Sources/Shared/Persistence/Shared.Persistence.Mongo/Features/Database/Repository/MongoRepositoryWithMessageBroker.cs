using System.Reactive.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Shared.Features;
using Shared.MessageBroker.Core;

namespace Shared.Persistence.Mongo.Features.Database.Repository;

internal class MongoRepositoryWithMessageBroker<T> : MongoRepository<T>, IDisposable where T : class, IDocument<string>
{
    private readonly IDisposable    _sub;

    public MongoRepositoryWithMessageBroker(IMongoClient client, ILogger<MongoRepositoryWithMessageBroker<T>> logger, IConfiguration configuration, IMessageBroker messageBroker) : base(client, logger, configuration)
    {
        var docChangesObserver = messageBroker.GetObserverForQueue<DocumentChangeBase<T, string>>(MessageBroker.Core.MessageBroker.GetQueueName<T>());
        
        _sub = DocumentChangedObservable.Cast<DocumentChangeBase<T, string>>().Concat(DocumentsChangedObservable.Cast<DocumentChangeBase<T, string>>())
                   .Do(docChangesObserver)
                   .Subscribe();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _sub.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}