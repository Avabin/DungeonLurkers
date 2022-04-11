using PierogiesBot.Persistence.Guild.Features.Many;
using PierogiesBot.Persistence.Guild.Features.Single;
using PierogiesBot.Shared.Features.Guilds;
using Shared.Persistence.Core.Features.Documents.Many;

namespace PierogiesBot.Persistence.Guild.Features;

internal class GuildFacade : DocumentFacade<GuildDocument, string, GuildDto>, IGuildFacade
{
    private readonly ISingleGuildService _singleDocumentService;

    public GuildFacade(ISingleGuildService singleDocumentService, IManyGuildsService manyDocumentsService) : base(singleDocumentService, manyDocumentsService)
    {
        _singleDocumentService     = singleDocumentService;
    }

    public async Task<GuildSubscribedChannelsDto> GetAllSubscribedChannelsAsync(string id)
    {
        var channels = await _singleDocumentService.GetAllSubscribedChannelsAsync(id);
        
        return new GuildSubscribedChannelsDto
        {
            GuildId = id,
            SubscribedChannels = channels.ToList()
        };
    }

    public async Task AddSubscribedChannelAsync(string id, ulong channelId) =>
        await _singleDocumentService.AddSubscribedChannelAsync(id, channelId);

    public async Task RemoveSubscribedChannelAsync(string id, ulong channelId) => 
        await _singleDocumentService.RemoveSubscribedChannelAsync(id, channelId);

    public async Task<GuildSubscribedRulesDto> GetAllSubscribedCrontabRulesAsync(string id)
    {
        var rules =  await _singleDocumentService.GetAllSubscribedCrontabRulesAsync(id);
        
        return new GuildSubscribedRulesDto()
        {
            GuildId            = id,
            SubscribedRules = rules.ToList(),
            RuleType = RuleType.Scheduled
        };
    }

    public async Task EnableCrontabRuleAsync(string id, string crontabRuleId) => 
        await _singleDocumentService.AddCrontabRuleToGuildAsync(id, crontabRuleId);

    public async Task DisableCrontabRuleAsync(string id, string crontabRuleId) => 
        await _singleDocumentService.RemoveCrontabRuleFromGuildAsync(id, crontabRuleId);

    public async Task<GuildSubscribedRulesDto> GetAllSubscribedResponseRulesAsync(string id)
    {
        var rules =  await _singleDocumentService.GetAllSubscribedResponseRulesAsync(id);
        
        return new GuildSubscribedRulesDto()
        {
            GuildId         = id,
            SubscribedRules = rules.ToList(),
            RuleType        = RuleType.Response
        };
    }

    public async Task EnableResponseRuleAsync(string id, string responseRuleId) => 
        await _singleDocumentService.AddResponseRuleToGuildAsync(id, responseRuleId);

    public async Task DisableResponseRuleAsync(string id, string responseRuleId) => 
        await _singleDocumentService.RemoveResponseRuleFromGuildAsync(id, responseRuleId);

    public async Task<GuildSubscribedRulesDto> GetAllSubscribedReactionRulesAsync(string id)
    {
        var rules =  await _singleDocumentService.GetAllSubscribedReactionRulesAsync(id);
        
        return new GuildSubscribedRulesDto()
        {
            GuildId         = id,
            SubscribedRules = rules.ToList(),
            RuleType        = RuleType.Reaction
        };
    }

    public async Task EnableReactionRuleAsync(string id, string reactionRuleId) => 
        await _singleDocumentService.AddReactionRuleToGuildAsync(id, reactionRuleId);

    public async Task DisableReactionRuleAsync(string id, string reactionRuleId) => 
        await _singleDocumentService.RemoveReactionRuleFromGuildAsync(id, reactionRuleId);
}