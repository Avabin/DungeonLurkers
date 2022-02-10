using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.MessageBroker.Core;

namespace Shared.MessageBroker.RabbitMQ;

// ReSharper disable once InconsistentNaming
public class RabbitMQMessageBroker : IMessageBroker
{
    private readonly ILogger<RabbitMQMessageBroker> _logger;
    private readonly IOptions<RabbitMqSettings>     _options;
    private          RabbitMqSettings               Settings => _options.Value;

    protected        IConnectionFactory       ConnectionFactory => _connectionFactory.Value;
    private readonly Lazy<IConnectionFactory> _connectionFactory;
    protected        IConnection              Connection => _connection.Value;
    private readonly Lazy<IConnection>        _connection;

    private readonly JsonSerializerSettings _serializerSettings = new()
    {
        TypeNameHandling               = TypeNameHandling.Objects,
        TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
    };
    public RabbitMQMessageBroker(ILogger<RabbitMQMessageBroker> logger, IOptions<RabbitMqSettings> options)
    {
        _logger       = logger;
        _options = options;
        _connectionFactory = new Lazy<IConnectionFactory>(() =>
        {
            var hostName = Settings.Host;
            _logger.LogTrace("Creating RabbitMQ Connection Factory for host {HostName}", hostName);
            return new ConnectionFactory() { HostName = hostName };
        });
        _connection = new Lazy<IConnection>(() =>
        {
            _logger.LogTrace("Creating RabbitMQ connection");
            return ConnectionFactory.CreateConnection();
        });
    }
    
    public IObservable<T> GetObservableForQueue<T>(string queueName) =>
        new ChannelObservable<T>(queueName, CreateQueueChannel, _serializerSettings);
    
    public IObserver<T> GetObserverForQueue<T>(string queueName) =>
        new ChannelObserver<T>(queueName, CreateQueueChannel, _serializerSettings);
    
    private IModel CreateQueueChannel(string queueName)
    {
        _logger.LogTrace("Creating channel for exchange {QueueName}", queueName);
        var channel = Connection.CreateModel();
        _logger.LogTrace("Declaring queue {QueueName}", queueName);
        channel.QueueDeclare(queueName, false, false, false, null);
        return channel;
    }
}