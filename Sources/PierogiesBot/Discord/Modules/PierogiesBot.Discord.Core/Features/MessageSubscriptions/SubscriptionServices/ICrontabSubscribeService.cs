using Discord;

namespace PierogiesBot.Discord.Core.Features.MessageSubscriptions.SubscriptionServices;

public interface ICrontabSubscribeService : ILoadSubscriptions
{
    Task SubscribeAsync(IGuildChannel channel);
    Task UnsubscribeAsync(IGuildChannel channel);
}