using Shared.Features;

namespace Shared.MessageBroker.Core;

public abstract record DocumentChangeBase<TDocument, TId>(TDocument OldDocument, bool HasMany, IEnumerable<TDocument> OldDocuments, ChangeType ChangeType)
: IMessage
    where TDocument : class, IDocument<TId>
{
    public Guid CorrelationId { get; } = Guid.NewGuid();
}