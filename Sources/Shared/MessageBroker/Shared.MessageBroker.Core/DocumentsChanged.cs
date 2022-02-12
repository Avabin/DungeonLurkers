using Shared.Features;

namespace Shared.MessageBroker.Core;

public record DocumentsChanged<TDocument, TId>(Snapshot<IEnumerable<TDocument>> DocumentsSnapshot,
                                               IEnumerable<TId>                 ModifiedIds, ChangeType ChangeType)
    : DocumentChangeBase<TDocument, TId>(null, true, DocumentsSnapshot, ChangeType)
    where TDocument : class, IDocument<TId>
{
    public IEnumerable<TId> DocumentIds => ModifiedIds;
}

public static class DocumentsChanged
{
    public static DocumentsChanged<TDocument, string> Inserted<TDocument>(IEnumerable<TDocument> newDocuments)
        where TDocument : class, IDocument<string> => Inserted<TDocument, string>(newDocuments);

    public static DocumentsChanged<TDocument, TId> Inserted<TDocument, TId>(IEnumerable<TDocument> newDocuments)
        where TDocument : class, IDocument<TId>
    {
        var docs              = newDocuments.ToList();
        var documentsSnapshot = Snapshot.Of<IEnumerable<TDocument>>(current: docs);
        var modifiedIds       = docs.Select(x => x.Id).ToList();
        return new DocumentsChanged<TDocument, TId>(documentsSnapshot, modifiedIds, ChangeType.Insert);
    }

    public static DocumentsChanged<TDocument, string> Updated<TDocument>(IEnumerable<TDocument> oldDocuments, IEnumerable<TDocument> newDocuments)
        where TDocument : class, IDocument<string> => Updated<TDocument, string>(oldDocuments, newDocuments);

    public static DocumentsChanged<TDocument, TId> Updated<TDocument, TId>(IEnumerable<TDocument> oldDocuments, IEnumerable<TDocument> newDocuments)
        where TDocument : class, IDocument<TId>
    {
        var docs              = newDocuments.ToList();
        var documentsSnapshot = Snapshot.Of(oldDocuments, docs);
        var modifiedIds       = docs.Select(x => x.Id).ToList();
        return new DocumentsChanged<TDocument, TId>(documentsSnapshot, modifiedIds, ChangeType.Update);
    }

    public static DocumentsChanged<TDocument, string> Deleted<TDocument>(IEnumerable<TDocument> oldDocuments)
        where TDocument : class, IDocument<string> => Deleted<TDocument, string>(oldDocuments);

    public static DocumentsChanged<TDocument, TId> Deleted<TDocument, TId>(IEnumerable<TDocument> oldDocuments)
        where TDocument : class, IDocument<TId>
    {
        var docs              = oldDocuments.ToList();
        var documentsSnapshot = Snapshot.Of<IEnumerable<TDocument>>(docs);
        var modifiedIds       = docs.Select(x => x.Id).ToList();
        return new DocumentsChanged<TDocument, TId>(documentsSnapshot, modifiedIds, ChangeType.Delete);
    }
}