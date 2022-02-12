using Shared.Features;

namespace Shared.MessageBroker.Core;

public record DocumentChanged<TDocument, TId>(Snapshot<TDocument> DocumentSnapshot, TId DocumentId,ChangeType ChangeType) 
    : DocumentChangeBase<TDocument, TId>(DocumentSnapshot, false, Snapshot.Of<IEnumerable<TDocument>>(Enumerable.Empty<TDocument>(), Enumerable.Empty<TDocument>()), ChangeType)
    where TDocument : class, IDocument<TId>
{
}

public static class DocumentChanged
{
    public static DocumentChanged<TDocument, string> Inserted<TDocument>(TDocument newDocument) where TDocument : class, IDocument<string> =>
        new(Snapshot.Of(current: newDocument), newDocument.Id, ChangeType.Insert);
    public static DocumentChanged<TDocument, TId> Inserted<TDocument, TId>(TDocument newDocument) where TDocument : class, IDocument<TId> =>
        new(Snapshot.Of(current: newDocument), newDocument.Id, ChangeType.Insert);

    public static DocumentChanged<TDocument, string> Updated<TDocument>(TDocument oldDocument, TDocument newDocument) where TDocument : class, IDocument<string> =>
        new(Snapshot.Of(oldDocument, newDocument), oldDocument.Id, ChangeType.Update);
    public static DocumentChanged<TDocument, TId> Updated<TDocument, TId>(TDocument oldDocument, TDocument newDocument) where TDocument : class, IDocument<TId> =>
        new(Snapshot.Of(oldDocument, newDocument), oldDocument.Id, ChangeType.Update);
    
    public static DocumentChanged<TDocument, string> Deleted<TDocument>(TDocument oldDocument) where TDocument : class, IDocument<string> =>
        new(Snapshot.Of(previous: oldDocument), oldDocument.Id, ChangeType.Delete);
    
    public static DocumentChanged<TDocument, TId> Deleted<TDocument, TId>(TDocument oldDocument) where TDocument : class, IDocument<TId> =>
        new(Snapshot.Of(previous: oldDocument), oldDocument.Id, ChangeType.Delete);
}