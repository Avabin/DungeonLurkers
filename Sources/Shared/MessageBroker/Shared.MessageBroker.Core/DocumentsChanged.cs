using Shared.Features;

namespace Shared.MessageBroker.Core;

public record DocumentsChanged<TDocument, TId>(IEnumerable<TDocument> OldDocuments, ChangeType ChangeType = ChangeType.Insert)
    : DocumentChangeBase<TDocument, TId>(null, true, OldDocuments, ChangeType)
    where TDocument : class, IDocument<TId>
{
    public IEnumerable<TId> DocumentIds => OldDocuments.Select(x => x.Id);
}

public static class DocumentsChanged
{
    public static DocumentsChanged<TDocument, TId> Of<TDocument, TId>(IEnumerable<TDocument> oldDocuments,
                                                                      ChangeType changeType = ChangeType.Insert)
        where TDocument : class, IDocument<TId> =>
        new(oldDocuments, changeType);

    public static DocumentsChanged<TDocument, string> Of<TDocument>(IEnumerable<TDocument> oldDocuments,
                                                                    ChangeType changeType = ChangeType.Insert)
        where TDocument : class, IDocument<string> =>
        new(oldDocuments, changeType);

    public static DocumentsChanged<TDocument, string> Inserted<TDocument>(IEnumerable<TDocument> oldDocuments)
        where TDocument : class, IDocument<string> =>
        new(oldDocuments, ChangeType.Insert);

    public static DocumentsChanged<TDocument, TId> Inserted<TDocument, TId>(IEnumerable<TDocument> oldDocuments)
        where TDocument : class, IDocument<TId> =>
        new(oldDocuments, ChangeType.Insert);

    public static DocumentsChanged<TDocument, string> Updated<TDocument>(IEnumerable<TDocument> oldDocuments)
        where TDocument : class, IDocument<string> =>
        new(oldDocuments, ChangeType.Update);

    public static DocumentsChanged<TDocument, TId> Updated<TDocument, TId>(IEnumerable<TDocument> oldDocuments)
        where TDocument : class, IDocument<TId> =>
        new(oldDocuments, ChangeType.Update);

    public static DocumentsChanged<TDocument, string> Deleted<TDocument>(IEnumerable<TDocument> oldDocuments)
        where TDocument : class, IDocument<string> =>
        new(oldDocuments, ChangeType.Delete);

    public static DocumentsChanged<TDocument, TId> Deleted<TDocument, TId>(IEnumerable<TDocument> oldDocuments)
        where TDocument : class, IDocument<TId> =>
        new(oldDocuments, ChangeType.Delete);
}