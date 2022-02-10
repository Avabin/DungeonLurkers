using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Shared.MessageBroker.Core;

public class DummyMessageBroker : IMessageBroker
{
    private readonly ISubject<object> _subject = new Subject<object>();
    public           IObservable<T>   GetObservableForQueue<T>(string queueName) => _subject.OfType<T>().AsObservable();

    public IObserver<T> GetObserverForQueue<T>(string queueName) => (IObserver<T>) _subject.AsObserver();
}