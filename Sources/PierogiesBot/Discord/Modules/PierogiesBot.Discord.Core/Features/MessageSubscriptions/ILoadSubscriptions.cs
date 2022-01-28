namespace PierogiesBot.Discord.Core.Features.MessageSubscriptions;

public interface ILoadSubscriptions
{
    /// <summary>
    /// Loads all saved subscriptions.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    Task LoadSubscriptionsAsync();
}