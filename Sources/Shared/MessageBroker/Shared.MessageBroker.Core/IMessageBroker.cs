namespace Shared.MessageBroker.Core;

public interface IMessageBroker
{
    IObservable<T>                    GetObservableForQueue<T>(string queueName);
    IObserver<T>                      GetObserverForQueue<T>(string   queueName);
}