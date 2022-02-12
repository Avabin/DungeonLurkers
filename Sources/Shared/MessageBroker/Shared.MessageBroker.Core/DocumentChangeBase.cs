using Shared.Features;

namespace Shared.MessageBroker.Core;
public abstract record DocumentChangeBase<TDocument, TId>(Snapshot<TDocument>? SingleDocumentSnapshot, bool HasMany, Snapshot<IEnumerable<TDocument>>? ManyDocumentsSnapshot, ChangeType ChangeType)
    : IMessage
    where TDocument : class, IDocument<TId>
{
    public Guid CorrelationId { get; } = Guid.NewGuid();
}