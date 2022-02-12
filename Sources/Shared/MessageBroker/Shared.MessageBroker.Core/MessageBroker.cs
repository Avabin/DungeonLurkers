namespace Shared.MessageBroker.Core;

public static class MessageBroker
{
    public static string GetQueueName<T>() =>
        typeof(T).Name + "Changed";
}