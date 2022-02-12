using System.Reactive.Linq;
using AutoMapper;
using Shared.Features;
using Shared.MessageBroker.Core;
using Shared.Persistence.Core.Features.Documents;
using Shared.Persistence.Core.Features.Documents.Single;
using Shared.Persistence.Core.Features.Repository;

namespace Shared.Persistence.Mongo.Features.Database.Documents.Single;

public class MongoSingleDocumentService<TDocument, TFindDocumentDto>
    : SingleDocumentService<TDocument, string, TFindDocumentDto>,
      IMongoSingleDocumentService<TDocument, TFindDocumentDto>,
      IDisposable
    where TDocument : class, IDocument<string>
    where TFindDocumentDto : IDocumentDto<string>
{
    private readonly IDisposable    _sub;

    public MongoSingleDocumentService(IRepository<TDocument, string> repository, IMapper mapper, IMessageBroker messageBroker) : base(repository, mapper)
    {
        var documentChangedObserver = messageBroker.GetObserverForQueue<DocumentChanged<TDocument, string>>(typeof(TDocument).Name + "Changed");

        _sub = Repository.DocumentChangedObservable.Do(documentChangedObserver).Subscribe();
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