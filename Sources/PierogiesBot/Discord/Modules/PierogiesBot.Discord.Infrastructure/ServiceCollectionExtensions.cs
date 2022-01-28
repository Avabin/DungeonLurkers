using Autofac;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PierogiesBot.Discord.Core.Features.MessageSubscriptions.Handlers;
using PierogiesBot.Discord.Core.Features.MessageSubscriptions.SubscriptionServices;
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


    public static ContainerBuilder AddDiscordServices(this ContainerBuilder services)
    {
        services.RegisterType<DiscordService>().AsSelf().AsImplementedInterfaces().SingleInstance();
        services.RegisterType<DiscordHostedService>().As<IHostedService>();
        services.RegisterType<DiscordSocketClient>().AsSelf().AsImplementedInterfaces().SingleInstance();
        services.RegisterType<ChannelSubscribeService>().AsImplementedInterfaces();
        services.RegisterType<CrontabSubscribeService>().AsImplementedInterfaces();
        services.RegisterType<BotSubscriptionRuleMessageHandler>().AsImplementedInterfaces();
        services.RegisterType<BotReactionsMessageHandler>().AsImplementedInterfaces();
        services.RegisterType<BotResponseMessageHandler>().AsImplementedInterfaces();

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