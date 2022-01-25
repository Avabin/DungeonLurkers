using Microsoft.Extensions.DependencyInjection;
using PierogiesBot.Discord.Commands.Features.MessageSubscriptions.Handlers;
using PierogiesBot.Discord.Commands.Features.MessageSubscriptions.SubscriptionServices;

namespace PierogiesBot.Discord.Commands;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDiscordCommandServices(this IServiceCollection collection) =>
        collection.AddTransient<IChannelSubscribeService, ChannelSubscribeService>()
            .AddTransient<IRuleMessageHandler, BotSubscriptionRuleMessageHandler>()
            .AddTransient<IUserSocketMessageHandler, BotReactionsMessageHandler>()
            .AddTransient<IUserSocketMessageHandler, BotResponseMessageHandler>();
}