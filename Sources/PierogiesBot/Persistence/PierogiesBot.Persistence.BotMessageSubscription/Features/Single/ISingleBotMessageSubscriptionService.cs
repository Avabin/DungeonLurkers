using PierogiesBot.Shared.Enums;
using PierogiesBot.Shared.Features.BotMessageSubscriptions;
using Shared.Persistence.Core.Features.Documents.Single;

namespace PierogiesBot.Persistence.BotMessageSubscription.Features.Single;

public interface ISingleBotMessageSubscriptionService : ISingleDocumentService<BotMessageSubscriptionDocument, string, BotMessageSubscriptionDto>
{
    Task<BotMessageSubscriptionDto?> GetSubscriptionForChannelAsync(ulong channelId, ulong guildId, SubscriptionType subscriptionType);
}