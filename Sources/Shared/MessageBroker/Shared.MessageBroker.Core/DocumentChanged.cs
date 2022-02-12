using Shared.Features;

namespace Shared.MessageBroker.Core;

public record DocumentChanged<TDocument, TId>(TDocument OldDocument, ChangeType ChangeType = ChangeType.Insert) 
    : DocumentChangeBase<TDocument, TId>(OldDocument, false, Enumerable.Empty<TDocument>(),ChangeType)
    where TDocument : class, IDocument<TId>
{
    public TId DocumentId => OldDocument.Id;
}

public static class DocumentChanged
{
    public static DocumentChanged<TDocument, TId> Of<TDocument, TId>(TDocument oldDocument, ChangeType changeType = ChangeType.Insert) where TDocument : class, IDocument<TId> =>
        new(oldDocument, changeType);
    public static DocumentChanged<TDocument, string> Of<TDocument>(TDocument oldDocument, ChangeType changeType = ChangeType.Insert) where TDocument : class, IDocument<string> =>
        new(oldDocument, changeType);
    
    public static DocumentChanged<TDocument, string> Inserted<TDocument>(TDocument oldDocument) where TDocument : class, IDocument<string> =>
        new(oldDocument, ChangeType.Insert);
    public static DocumentChanged<TDocument, TId> Inserted<TDocument, TId>(TDocument oldDocument) where TDocument : class, IDocument<TId> =>
        new(oldDocument, ChangeType.Insert);

    public static DocumentChanged<TDocument, string> Updated<TDocument>(TDocument oldDocument) where TDocument : class, IDocument<string> =>
        new(oldDocument, ChangeType.Update);
    public static DocumentChanged<TDocument, TId> Updated<TDocument, TId>(TDocument oldDocument) where TDocument : class, IDocument<TId> =>
        new(oldDocument, ChangeType.Update);
    
    public static DocumentChanged<TDocument, string> Deleted<TDocument>(TDocument oldDocument) where TDocument : class, IDocument<string> =>
        new(oldDocument, ChangeType.Delete);
    
    public static DocumentChanged<TDocument, TId> Deleted<TDocument, TId>(TDocument oldDocument) where TDocument : class, IDocument<TId> =>
        new(oldDocument, ChangeType.Delete);
}