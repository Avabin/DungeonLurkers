using Discord.WebSocket;

namespace PierogiesBot.Discord.Commands.Features.MessageSubscriptions.SubscriptionServices;

public interface ICrontabSubscribeService
{
    Task SubscribeAsync(SocketGuildChannel channel);
    Task UnsubscribeAsync(SocketTextChannel channel);
}