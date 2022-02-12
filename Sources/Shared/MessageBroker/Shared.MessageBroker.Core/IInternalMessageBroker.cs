using Shared.Features;

namespace Shared.MessageBroker.Core;

public interface IInternalMessageBroker : IMessageBroker
{
    IObservable<DocumentChangeBase<T, string>> GetDocumentChangesObservable<T>() where T : class, IDocument<string> =>
        GetObservableForQueue<DocumentChangeBase<T, string>>(MessageBroker.GetQueueName<T>());
    
    IObserver<DocumentChangeBase<T, string>> GetDocumentChangesObserver<T>() where T : class, IDocument<string> =>
        GetObserverForQueue<DocumentChangeBase<T, string>>(MessageBroker.GetQueueName<T>());
}