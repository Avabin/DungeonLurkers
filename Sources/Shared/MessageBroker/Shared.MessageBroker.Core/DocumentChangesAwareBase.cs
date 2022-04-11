using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Shared.Features;

namespace Shared.MessageBroker.Core;

public abstract class DocumentChangesAwareBase<T> where T : class, IDocument<string>
{
    private readonly IDocumentMessageBroker _messageBroker;
    private  IDisposable?           _sub;

    protected DocumentChangesAwareBase(IDocumentMessageBroker messageBroker)
    {
        _messageBroker = messageBroker;
    }
    private async Task HandleDocumentChangeAsync(DocumentChangeBase<T, string> change)
    {
        switch (change)
        {
            case DocumentChanged<T, string> x:
                await HandleDocumentChangedAsync(x);
                break;
            case DocumentsChanged<T, string> x:
                await HandleDocumentsChangedAsync(x);
                break;
        }
    }

    private async Task HandleDocumentChangedAsync(DocumentChanged<T,string> documentChanged)
    {
        var (previous, current) = documentChanged.DocumentSnapshot;
        switch (documentChanged.ChangeType)
        {
            case ChangeType.Insert:
                await HandleDocumentInsertedAsync(current!);
                break;
            case ChangeType.Delete:
                await HandleDocumentDeletedAsync(previous!);
                break;
            case ChangeType.Update:
                await HandleDocumentUpdatedAsync(previous!, current!);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(documentChanged));
        }
    }

    protected abstract Task HandleDocumentUpdatedAsync(T  oldDocument, T updatedDocument);
    protected abstract Task HandleDocumentDeletedAsync(T  deletedDocument);
    protected abstract Task HandleDocumentInsertedAsync(T newDocument);

    private async Task HandleDocumentsChangedAsync(DocumentsChanged<T,string> documentsChanged)
    {
        var (previous, current) = documentsChanged.DocumentsSnapshot;

        switch (documentsChanged.ChangeType)
        {
            case ChangeType.Insert:
                await HandleDocumentsInsertedAsync(current!);
                break;
            case ChangeType.Delete:
                await HandleDocumentsDeletedAsync(previous!);
                break;
            case ChangeType.Update:
                await HandleDocumentsUpdatedAsync(previous!, current!);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(documentsChanged));
        }
    }

    protected virtual async Task HandleDocumentsInsertedAsync(IEnumerable<T> inserted)
    {
        foreach (var d in inserted)
            await HandleDocumentInsertedAsync(d);
    }

    protected virtual async Task HandleDocumentsDeletedAsync(IEnumerable<T> deleted)
    {
        foreach (var d in deleted)
            await HandleDocumentDeletedAsync(d);
    }

    protected virtual async Task HandleDocumentsUpdatedAsync(IEnumerable<T> previous, IEnumerable<T> current)
    {
        foreach (var (p, c) in previous.Zip(current))
            await HandleDocumentUpdatedAsync(p, c);
    }

    public void Start()
    {
        _sub = _messageBroker.GetDocumentChangesObservable<T>()
                            .Select(x => Observable.Defer(() => HandleDocumentChangeAsync(x).ToObservable()))
                            .Concat()
                            .Subscribe();
    }

    public void Stop()
    {
        _sub?.Dispose();
    }
}