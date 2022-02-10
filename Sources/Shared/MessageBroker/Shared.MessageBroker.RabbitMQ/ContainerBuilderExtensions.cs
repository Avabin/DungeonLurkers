using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.MessageBroker.Core;

namespace Shared.MessageBroker.RabbitMQ;

public static class ContainerBuilderExtensions
{
    
    public static IServiceCollection ConfigureRabbit(this IServiceCollection services, IConfigurationSection section)
    {
        services.Configure<RabbitMqSettings>(section);

        return services;
    }
    
    public static IServiceCollection ConfigureRabbit(this IServiceCollection services, Action<RabbitMqSettings> configure)
    {
        services.Configure(configure);

        return services;
    }
    public static ContainerBuilder AddRabbitMessageBroker(this ContainerBuilder builder)
    {
        builder.RegisterType<RabbitMQMessageBroker>().As<IMessageBroker>().SingleInstance();

        return builder;
    }
    
    public static ContainerBuilder AddDummyMessageBroker(this ContainerBuilder builder)
    {
        builder.RegisterType<DummyMessageBroker>().As<IMessageBroker>().SingleInstance();

        return builder;
    }
}