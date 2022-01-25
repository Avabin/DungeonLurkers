using AutoMapper;
using PierogiesBot.Shared.Enums;
using PierogiesBot.Shared.Features.BotMessageSubscriptions;
using Shared.Persistence.Core.Features.Documents.Many;
using Shared.Persistence.Core.Features.Repository;

namespace PierogiesBot.Persistence.BotMessageSubscription.Features.Many;

internal class ManyBotMessageSubscriptionsService
    : ManyDocumentsService<BotMessageSubscriptionDocument, string, BotMessageSubscriptionDto>,
      IManyBotMessageSubscriptionsService
{
    public ManyBotMessageSubscriptionsService(IRepository<BotMessageSubscriptionDocument, string> repository,
                                              IMapper                                             mapper)
        : base(repository, mapper)
    {
    }

    public async Task<IReadOnlyCollection<BotMessageSubscriptionDto>> GetByGuildAndSubscriptionTypeAsync(
        ulong guildId, SubscriptionType subscriptionType)
    {
        var subs =
            await Repository.GetAllByPredicateAsync(x => x.GuildId          == guildId
                                                      && x.SubscriptionType == subscriptionType);

        return Mapper.Map<IReadOnlyCollection<BotMessageSubscriptionDto>>(subs);
    }

    public async Task<IEnumerable<BotMessageSubscriptionDto>> GetAllSubscriptionsForChannelAsync(
        ulong channelId, ulong guildId)
    {
        var subs = await Repository.GetAllByPredicateAsync(x => x.ChannelId == channelId && x.GuildId == guildId);

        return Mapper.Map<IEnumerable<BotMessageSubscriptionDto>>(subs);
    }

    public async Task<IEnumerable<BotMessageSubscriptionDto>> GetAllSubscriptionsForGuildAsync(ulong guildId)
    {
        var subs = await Repository.GetAllByPredicateAsync(x => x.GuildId == guildId);
        
        return Mapper.Map<IEnumerable<BotMessageSubscriptionDto>>(subs);
    }
}