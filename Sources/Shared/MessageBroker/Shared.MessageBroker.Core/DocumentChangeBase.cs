using Shared.Features;

namespace Shared.MessageBroker.Core;

public abstract record DocumentChangeBase<TDocument, TId>(TDocument OldDocument, bool HasMany, IEnumerable<TDocument> OldDocuments, ChangeType ChangeType)
    where TDocument : class, IDocument<TId>
{
}