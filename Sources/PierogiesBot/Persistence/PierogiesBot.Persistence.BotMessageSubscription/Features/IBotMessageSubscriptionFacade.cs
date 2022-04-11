using PierogiesBot.Shared.Enums;
using PierogiesBot.Shared.Features.BotMessageSubscriptions;
using Shared.Persistence.Core.Features.Documents.Many;

namespace PierogiesBot.Persistence.BotMessageSubscription.Features;

public interface
    IBotMessageSubscriptionFacade : IDocumentFacade<BotMessageSubscriptionDocument, string,
        BotMessageSubscriptionDto>
{
    Task<BotMessageSubscriptionDto?> GetSubscriptionForChannelAsync(ulong            channelId, ulong guildId,
                                                                    SubscriptionType responses);
    Task<IEnumerable<BotMessageSubscriptionDto>> GetAllSubscriptionsForChannelAsync(ulong channelId, ulong guildId);

    Task<IEnumerable<BotMessageSubscriptionDto>> GetByGuildAndSubscriptionTypeAsync(
        ulong guildId, SubscriptionType crontab);

    Task<IEnumerable<BotMessageSubscriptionDto>> GetAllSubscriptionsForGuildAsync(ulong guildId);
}