namespace Shared.MessageBroker.Core;

public interface IMessageBroker
{
    IObservable<T>                    GetObservableForQueue<T>(string queueName) where T : IMessage;
    public IObservable<T> GetObservableForQueue<T>() where T : IMessage =>
        GetObservableForQueue<T>(MessageBroker.GetQueueName<T>());
    IObserver<T>                      GetObserverForQueue<T>(string   queueName) where T : IMessage;
    public IObserver<T> GetObserverForQueue<T>() where T : IMessage => 
        GetObserverForQueue<T>(MessageBroker.GetQueueName<T>());
}