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
    public static ContainerBuilder AddInternalMessageBroker(this ContainerBuilder builder)
    {
        builder.RegisterType<DocumentMessageBroker>().AsImplementedInterfaces().SingleInstance();

        return builder;
    }
    
    public static ContainerBuilder AddRabbitMqMessageBroker(this ContainerBuilder builder)
    {
        builder.RegisterType<RabbitMQMessageBroker>().AsImplementedInterfaces().SingleInstance();

        return builder;
    }
}