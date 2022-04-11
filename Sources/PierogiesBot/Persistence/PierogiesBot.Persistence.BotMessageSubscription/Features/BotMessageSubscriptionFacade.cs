using PierogiesBot.Persistence.BotMessageSubscription.Features.Many;
using PierogiesBot.Persistence.BotMessageSubscription.Features.Single;
using PierogiesBot.Shared.Enums;
using PierogiesBot.Shared.Features.BotMessageSubscriptions;
using Shared.Persistence.Core.Features.Documents.Many;

namespace PierogiesBot.Persistence.BotMessageSubscription.Features;

internal class BotMessageSubscriptionFacade : DocumentFacade<BotMessageSubscriptionDocument, string, BotMessageSubscriptionDto>, IBotMessageSubscriptionFacade
{
    private readonly ISingleBotMessageSubscriptionService _singleDocumentService;
    private readonly IManyBotMessageSubscriptionsService  _manyDocumentsService;

    public BotMessageSubscriptionFacade(
        ISingleBotMessageSubscriptionService singleDocumentService,
        IManyBotMessageSubscriptionsService  manyDocumentsService) :
        base(singleDocumentService, manyDocumentsService)
    {
        _singleDocumentService     = singleDocumentService;
        _manyDocumentsService = manyDocumentsService;
    }

    public async Task<BotMessageSubscriptionDto?> GetSubscriptionForChannelAsync(ulong channelId, ulong guildId, SubscriptionType responses) => 
        await _singleDocumentService.GetSubscriptionForChannelAsync(channelId, guildId, responses);

    public Task<IEnumerable<BotMessageSubscriptionDto>> GetAllSubscriptionsForChannelAsync(ulong channelId, ulong guildId)
    {
        return _manyDocumentsService.GetAllSubscriptionsForChannelAsync(channelId, guildId);
    }

    public async Task<IEnumerable<BotMessageSubscriptionDto>> GetByGuildAndSubscriptionTypeAsync(ulong guildId, SubscriptionType crontab) => 
        await _manyDocumentsService.GetByGuildAndSubscriptionTypeAsync(guildId, crontab);

    public async Task<IEnumerable<BotMessageSubscriptionDto>> GetAllSubscriptionsForGuildAsync(ulong guildId) => 
        await _manyDocumentsService.GetAllSubscriptionsForGuildAsync(guildId);
}