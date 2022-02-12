using System.Collections.Concurrent;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Shared.MessageBroker.Core;

public class InternalMessageBroker : IInternalMessageBroker
{
    private readonly ConcurrentDictionary<string, object> _queues = new();
    public IObservable<T> GetObservableForQueue<T>(string queueName) where T : IMessage
    {
        if (!_queues.TryGetValue(queueName, out var subject) || subject is not Subject<T> sub)
        {
            var newSubject = new Subject<T>();
            _queues.TryAdd(queueName, newSubject);
            return newSubject.AsObservable();
        }
        else
        {
            return sub.AsObservable();
        }
        return Observable.Empty<T>();
    }

    public IObserver<T> GetObserverForQueue<T>(string queueName) where T : IMessage
    {
        if (!_queues.TryGetValue(queueName, out var subject) || subject is not Subject<T> sub)
        {
            var newSubject = new Subject<T>();
            _queues.TryAdd(queueName, newSubject);
            return newSubject.AsObserver();
        }
        else
        {
            return sub.AsObserver();
        }
        return Observer.Create<T>(x => { });
    }
}