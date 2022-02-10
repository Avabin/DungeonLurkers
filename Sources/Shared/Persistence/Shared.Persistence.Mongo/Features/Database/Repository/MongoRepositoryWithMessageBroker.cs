using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Shared.Features;
using Shared.MessageBroker.Core;

namespace Shared.Persistence.Mongo.Features.Database.Repository;

internal class MongoRepositoryWithMessageBroker<T> : MongoRepository<T>, IDisposable where T : class, IDocument<string>
{
    private readonly IMessageBroker                           _messageBroker;
    private          IObserver<DocumentChangeBase<T, string>> _docChangesObserver;
    private          IDisposable                              _sub;

    public MongoRepositoryWithMessageBroker(IMongoClient client, ILogger<MongoRepositoryWithMessageBroker<T>> logger, IOptions<MongoSettings> options, IMessageBroker messageBroker) : base(client, logger, options)
    {
        _messageBroker = messageBroker;
        
        _docChangesObserver = _messageBroker.GetObserverForQueue<DocumentChangeBase<T, string>>(typeof(T).Name + "Changed");
        
        _sub = DocumentChangedObservable.Cast<DocumentChangeBase<T, string>>().Concat(DocumentsChangedObservable.Cast<DocumentChangeBase<T, string>>())
                   .Do(_docChangesObserver)
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