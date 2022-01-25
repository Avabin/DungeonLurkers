using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PierogiesBot.Discord.Commands.Features.MessageSubscriptions.SubscriptionServices;
using PierogiesBot.Discord.Infrastructure.Features.DiscordHost;
using PierogiesBot.Discord.Infrastructure.Features.MessageSubscriptions.Crontab;
using PierogiesBot.Discord.Infrastructure.Features.MessageSubscriptions.Handlers;
using Quartz;
using ICrontabSubscribeService = PierogiesBot.Discord.Infrastructure.Features.MessageSubscriptions.Crontab.ICrontabSubscribeService;

namespace PierogiesBot.Discord.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDiscord(this IServiceCollection services, IConfigurationSection discordSection) =>
        services.Configure<DiscordSettings>(discordSection)
            .AddCoreServices();
    public static IServiceCollection AddDiscord(this IServiceCollection services, Action<DiscordSettings> configure) =>
        services
            .Configure(configure)
            .AddCoreServices();

    private static IServiceCollection AddCoreServices(this IServiceCollection services) =>
        services
            .AddSingleton<IDiscordService, DiscordService>()
            .AddHostedService<DiscordHostedService>()
            .AddSingleton<IDiscordClient, DiscordSocketClient>()
            .AddSingleton(sp => sp.GetRequiredService<DiscordSocketClient>())
            .AddTransient<IChannelSubscribeService, ChannelSubscribeService>()
            .AddTransient<ICrontabSubscribeService, CrontabSubscribeService>()
            .AddTransient<IRuleMessageHandler, BotSubscriptionRuleMessageHandler>()
            .AddTransient<IUserSocketMessageHandler, BotReactionsMessageHandler>()
            .AddTransient<IUserSocketMessageHandler, BotResponseMessageHandler>()
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