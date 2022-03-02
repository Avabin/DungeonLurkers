using PierogiesBot.Shared.Features.Guilds;
using Shared.Persistence.Core.Features.Documents.Many;

namespace PierogiesBot.Persistence.Guild.Features;

public interface IGuildFacade : IDocumentFacade<GuildDocument, string, GuildDto>
{
    Task<GuildSubscribedChannelsDto> GetAllSubscribedChannelsAsync(string id);
    Task   AddSubscribedChannelAsync(string     id, ulong channelId);
    Task   RemoveSubscribedChannelAsync(string  id, ulong channelId);
    
    Task<GuildSubscribedRulesDto> GetAllSubscribedCrontabRulesAsync(string id);
    Task   EnableCrontabRuleAsync(string            id, string crontabRuleId);
    Task   DisableCrontabRuleAsync(string           id, string crontabRuleId);
    
    Task<GuildSubscribedRulesDto> GetAllSubscribedResponseRulesAsync(string id);
    Task   EnableResponseRuleAsync(string            id, string responseRuleId);
    Task   DisableResponseRuleAsync(string           id, string responseRuleId);
    
    Task<GuildSubscribedRulesDto> GetAllSubscribedReactionRulesAsync(string id);
    Task EnableReactionRuleAsync(string id, string reactionRuleId);
    Task DisableReactionRuleAsync(string id, string reactionRuleId);
}