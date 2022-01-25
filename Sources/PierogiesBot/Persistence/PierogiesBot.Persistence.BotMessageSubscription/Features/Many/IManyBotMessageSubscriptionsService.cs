using PierogiesBot.Shared.Enums;
using PierogiesBot.Shared.Features.BotMessageSubscriptions;
using Shared.Persistence.Core.Features.Documents.Many;

namespace PierogiesBot.Persistence.BotMessageSubscription.Features.Many;

public interface
    IManyBotMessageSubscriptionsService : IManyDocumentsService<BotMessageSubscriptionDocument, string,
        BotMessageSubscriptionDto>
{
    Task<IReadOnlyCollection<BotMessageSubscriptionDto>> GetByGuildAndSubscriptionTypeAsync(
        ulong guildId, SubscriptionType subscriptionType);

    Task<IEnumerable<BotMessageSubscriptionDto>> GetAllSubscriptionsForChannelAsync(ulong channelId, ulong guildId);
    Task<IEnumerable<BotMessageSubscriptionDto>> GetAllSubscriptionsForGuildAsync(ulong   guildId);
}