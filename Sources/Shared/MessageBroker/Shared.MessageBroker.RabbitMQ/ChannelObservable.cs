using System.Reactive.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Shared.MessageBroker.RabbitMQ;

public class ChannelObservable<T> : EventingBasicConsumer, IObservable<T>
{
    private readonly string                  _queueName;
    private readonly JsonSerializerSettings? _settings;

    public ChannelObservable(string                  queueName, Func<string, IModel> modelFactory,
                             JsonSerializerSettings? settings = null) : base(modelFactory(queueName))
    {
        _queueName = queueName;
        _settings       = settings;
        
        ReceivedObservable =
            Observable
               .FromEventPattern<BasicDeliverEventArgs>(this, nameof(Received))
               .Select(x => x.EventArgs)
               .Select(Transform);
    }

    protected IObservable<T> ReceivedObservable { get; }

    private T Transform(BasicDeliverEventArgs message)
    {
        using var ms         = new MemoryStream(message.Body.ToArray());
        using var bsonReader = new BsonDataReader(ms);
        var       serializer = JsonSerializer.CreateDefault(_settings);
        return serializer.Deserialize<T>(bsonReader);
    }

    public IDisposable Subscribe(IObserver<T> observer)
    {
        Model.BasicConsume(_queueName, autoAck: true, consumer: this);
        return ReceivedObservable.Subscribe(observer);
    }
}