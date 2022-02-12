using PierogiesBot.Persistence.BotMessageSubscription.Features.Many;
using PierogiesBot.Persistence.BotMessageSubscription.Features.Single;
using PierogiesBot.Shared.Enums;
using PierogiesBot.Shared.Features.BotMessageSubscriptions;
using Shared.Persistence.Core.Features.Documents.Many;

namespace PierogiesBot.Persistence.BotMessageSubscription.Features;

internal class BotMessageSubscriptionFacade : DocumentOperationFacade<BotMessageSubscriptionDocument, string, BotMessageSubscriptionDto>, IBotMessageSubscriptionFacade
{
    private readonly ISingleBotMessageSubscriptionService _singleSingleDocumentService;
    private readonly IManyBotMessageSubscriptionsService  _manyManyDocumentsService;

    public BotMessageSubscriptionFacade(
        ISingleBotMessageSubscriptionService singleSingleDocumentService,
        IManyBotMessageSubscriptionsService  manyManyDocumentsService) :
        base(singleSingleDocumentService, manyManyDocumentsService)
    {
        _singleSingleDocumentService     = singleSingleDocumentService;
        _manyManyDocumentsService = manyManyDocumentsService;
    }

    public async Task<BotMessageSubscriptionDto?> GetSubscriptionForChannelAsync(ulong channelId, ulong guildId, SubscriptionType responses) => 
        await _singleSingleDocumentService.GetSubscriptionForChannelAsync(channelId, guildId, responses);

    public Task<IEnumerable<BotMessageSubscriptionDto>> GetAllSubscriptionsForChannelAsync(ulong channelId, ulong guildId)
    {
        return _manyManyDocumentsService.GetAllSubscriptionsForChannelAsync(channelId, guildId);
    }

    public async Task<IEnumerable<BotMessageSubscriptionDto>> GetByGuildAndSubscriptionTypeAsync(ulong guildId, SubscriptionType crontab) => 
        await _manyManyDocumentsService.GetByGuildAndSubscriptionTypeAsync(guildId, crontab);

    public async Task<IEnumerable<BotMessageSubscriptionDto>> GetAllSubscriptionsForGuildAsync(ulong guildId) => 
        await _manyManyDocumentsService.GetAllSubscriptionsForGuildAsync(guildId);
}