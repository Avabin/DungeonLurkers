using Autofac;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PierogiesBot.Discord.Infrastructure.Features.DiscordHost;
using PierogiesBot.Discord.Infrastructure.Features.MessageSubscriptions;
using PierogiesBot.Discord.Infrastructure.Features.MessageSubscriptions.Crontab;
using PierogiesBot.Discord.Infrastructure.Features.MessageSubscriptions.Handlers;
using Quartz;

namespace PierogiesBot.Discord.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection
        AddDiscord(this IServiceCollection services, IConfigurationSection discordSection) =>
        services.Configure<DiscordSettings>(discordSection)
                .AddCoreServices();

    public static IServiceCollection AddDiscord(this IServiceCollection services, Action<DiscordSettings> configure) =>
        services
           .Configure(configure)
           .AddCoreServices();


    public static ContainerBuilder AddDiscordServices<TService>(this ContainerBuilder services) where TService : IDiscordService
    {
        services.RegisterType<TService>().AsSelf().As<IDiscordService>().AsImplementedInterfaces().SingleInstance();
        services.RegisterType<DiscordHostedService>().As<IHostedService>().SingleInstance();
        services.RegisterType<DiscordSocketClient>().AsSelf().AsImplementedInterfaces().SingleInstance();
        services.RegisterType<ChannelSubscribeService>().AsImplementedInterfaces().SingleInstance();
        services.RegisterType<CrontabSubscribeService>().AsImplementedInterfaces().SingleInstance();
        services.RegisterType<BotSubscriptionRuleMessageHandler>().AsImplementedInterfaces().SingleInstance();
        services.RegisterType<BotReactionsMessageHandler>().AsImplementedInterfaces().SingleInstance();
        services.RegisterType<BotResponseMessageHandler>().AsImplementedInterfaces().SingleInstance();
        services.RegisterType<SendCrontabMessageToChannelsJob>().AsSelf();

        return services;
    }

    private static IServiceCollection AddCoreServices(this IServiceCollection services) =>
        services
           .AddQuartz(configurator =>
            {
                configurator.SchedulerId = "CoreScheduler";

                configurator.UseMicrosoftDependencyInjectionJobFactory();
                configurator.UseSimpleTypeLoader();
                configurator.UseInMemoryStore();
            })
           .AddQuartzHostedService(options => options.WaitForJobsToComplete = false)
           .AddSingleton(serviceProvider =>
            {
                var config = SchedulerBuilder.Create();

                var scheduler = config.Build().GetScheduler().GetAwaiter().GetResult();

                scheduler.JobFactory = new DependencyInjectionJobFactory(serviceProvider);

                return scheduler;
            });
}