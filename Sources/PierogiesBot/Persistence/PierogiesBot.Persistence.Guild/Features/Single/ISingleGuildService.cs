using PierogiesBot.Shared.Features.Guilds;
using Shared.Persistence.Core.Features.Documents.Single;

namespace PierogiesBot.Persistence.Guild.Features.Single;

public interface ISingleGuildService : ISingleDocumentService<GuildDocument, string, GuildDto>
{
    Task<IEnumerable<ulong>>                    GetAllSubscribedChannelsAsync(string      id);
    Task                      AddSubscribedChannelAsync(string          id, ulong channelId);
    Task                      RemoveSubscribedChannelAsync(string       id, ulong channelId);
    Task<IEnumerable<string>> GetAllSubscribedCrontabRulesAsync(string  id);
    Task                      AddCrontabRuleToGuildAsync(string         id, string crontabRuleId);
    Task                      RemoveCrontabRuleFromGuildAsync(string    id, string crontabRuleId);
    Task<IEnumerable<string>> GetAllSubscribedResponseRulesAsync(string id);
    Task                      AddResponseRuleToGuildAsync(string        id, string responseRuleId);
    Task                      RemoveResponseRuleFromGuildAsync(string   id, string responseRuleId);
    Task<IEnumerable<string>> GetAllSubscribedReactionRulesAsync(string id);
    Task                      AddReactionRuleToGuildAsync(string        id, string reactionRuleId);
    Task                      RemoveReactionRuleFromGuildAsync(string   id, string reactionRuleId);
}