using Discord;

namespace PierogiesBot.Discord.Commands.Features.MessageSubscriptions.SubscriptionServices;

public interface IChannelSubscribeService
{
    /// <summary>
    /// Loads all saved subscriptions.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    Task LoadSubscriptionsAsync();
    
    /// <summary>
    /// Subscribes to given channel.
    /// </summary>
    /// <param name="channel">Channel to subscribe.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    Task SubscribeAsync(IGuildChannel channel);
    
    /// <summary>
    /// Unsubscribes from given channel.
    /// </summary>
    /// <param name="channel">Channel to unsubscribe.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    Task UnsubscribeAsync(IGuildChannel channel);
}