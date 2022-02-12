using RestEase;
using Shared.Features.Authentication;

namespace PierogiesBot.Shared.Features.BotMessageSubscriptions;

public interface IBotMessageSubscriptionApi : IAuthenticatedApi
{
    [Get("BotMessageSubscription")]
    Task<IEnumerable<BotMessageSubscriptionDto>> GetAllMessageSubscriptionsAsync([Query] int? skip = null, [Query] int? limit = null);

    [Get("BotMessageSubscription/{id}")] Task<BotMessageSubscriptionDto> FindMessageSubscriptionByIdAsync([Path] string id);
    
    [Get("BotMessageSubscription/guild/{guildId}/channel/{channelId}")]
    Task<IEnumerable<BotMessageSubscriptionDto>> FindAllMessageSubscriptionsForChannelAsync([Path] ulong guildId, [Path] ulong channelId);    
    [Get("BotMessageSubscription/guild/{guildId}")]
    Task<IEnumerable<BotMessageSubscriptionDto>> FindAllMessageSubscriptionsForGuildAsync([Path] ulong guildId);

    [Post("BotMessageSubscription")] Task<BotMessageSubscriptionDto> CreateBotMessageSubscriptionAsync([Body] CreateBotMessageSubscriptionDto createDto);

    [Put("BotMessageSubscription/{id}")]
    Task UpdateBotMessageSubscriptionAsync([Path] string id, [Body] UpdateBotMessageSubscriptionDto updateDto);

    [Delete("BotMessageSubscription/{id}")] Task DeleteBotMessageSubscriptionAsync([Path] string id);
}