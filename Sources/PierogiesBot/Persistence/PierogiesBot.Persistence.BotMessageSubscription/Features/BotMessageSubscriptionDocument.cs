using MongoDB.Bson;
using PierogiesBot.Shared.Enums;
using Shared.Persistence.Core.Features.Documents;

namespace PierogiesBot.Persistence.BotMessageSubscription.Features;

public record BotMessageSubscriptionDocument
    (string Id, ulong GuildId, ulong ChannelId, SubscriptionType SubscriptionType) : DocumentBase<string>(Id)
{
    public BotMessageSubscriptionDocument(ulong guildId, ulong channelId, SubscriptionType subscriptionType) : this(
     ObjectId.GenerateNewId().ToString(), guildId, channelId, subscriptionType)
    {
            
    }
}