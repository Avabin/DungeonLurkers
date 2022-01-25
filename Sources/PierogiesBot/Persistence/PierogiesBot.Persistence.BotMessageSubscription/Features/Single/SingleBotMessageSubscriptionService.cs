using AutoMapper;
using PierogiesBot.Shared.Enums;
using PierogiesBot.Shared.Features.BotMessageSubscriptions;
using Shared.Persistence.Core.Features.Documents.Single;
using Shared.Persistence.Core.Features.Repository;

namespace PierogiesBot.Persistence.BotMessageSubscription.Features.Single;

internal class SingleBotMessageSubscriptionService
    : SingleDocumentService<BotMessageSubscriptionDocument, string, BotMessageSubscriptionDto>, ISingleBotMessageSubscriptionService
{
    public SingleBotMessageSubscriptionService(IRepository<BotMessageSubscriptionDocument, string> repository, IMapper mapper) : base(repository,
        mapper)
    {
    }

    public async Task<BotMessageSubscriptionDto?> GetSubscriptionForChannelAsync(ulong channelId, ulong guildId, SubscriptionType subscriptionType)
    {
        var subscription = await Repository.GetByPredicateAsync(x => x.ChannelId == channelId && x.GuildId == guildId && x.SubscriptionType == subscriptionType);

        return subscription is null ? null : Mapper.Map<BotMessageSubscriptionDto>(subscription);
    }
}