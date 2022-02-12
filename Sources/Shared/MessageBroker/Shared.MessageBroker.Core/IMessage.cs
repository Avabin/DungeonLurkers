namespace Shared.MessageBroker.Core;

public interface IMessage
{
    Guid CorrelationId { get; }
}