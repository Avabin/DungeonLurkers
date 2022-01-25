using PierogiesBot.Shared.Features.BotMessageSubscriptions;
using Shared.Features;

namespace PierogiesBot.Persistence.BotMessageSubscription.Features;

public class PersistenceBotMessageSubscriptionsMapperProfile
    : DtoMapperProfile<BotMessageSubscriptionDto, CreateBotMessageSubscriptionDto, UpdateBotMessageSubscriptionDto, BotMessageSubscriptionDocument>
{
}