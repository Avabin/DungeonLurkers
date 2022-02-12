using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using RabbitMQ.Client;

namespace Shared.MessageBroker.RabbitMQ;

public class ChannelObserver<T> : IObserver<T>
{
    protected        string                  QueueName { get; }
    private readonly JsonSerializerSettings? _settings;
    
    private Lazy<IModel> _model { get; }
    public  IModel       Model  => _model.Value;

    public ChannelObserver(string queueName, Func<string, IModel> modelFactory, JsonSerializerSettings? settings = null)
    {
        QueueName = queueName;
        _settings  = settings;

        _model = new Lazy<IModel>(() => modelFactory(queueName));
    }

    public virtual void OnCompleted()
    {
        Model.Close();
        Model.Dispose();
    }
    
    public virtual void OnError(Exception error)
    {
        Model.Close(replyCode: 0xff, replyText: error.Message);
        Model.Dispose();
    }

    public virtual void OnNext(T value)
    {
        Console.WriteLine($"Sending message to queue {QueueName}");
        using var ms = new MemoryStream();
        using var bsonWriter = new BsonDataWriter(ms);
        var serializer = JsonSerializer.CreateDefault(_settings);
        serializer.Serialize(bsonWriter, value);
        var body = ms.ToArray();
        Model.BasicPublish("", QueueName, Model.CreateBasicProperties(), body);
    }
}
