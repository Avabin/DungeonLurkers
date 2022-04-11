using AutoMapper;
using PierogiesBot.Shared.Features.Guilds;
using Shared.Persistence.Core.Features.Documents.Single;
using Shared.Persistence.Core.Features.Repository;

namespace PierogiesBot.Persistence.Guild.Features.Single;

internal class SingleGuildService : SingleDocumentService<GuildDocument, string, GuildDto>, ISingleGuildService
{
    public SingleGuildService(IRepository<GuildDocument, string> repository, IMapper mapper) : base(repository, mapper)
    {
    }

    public async Task<IEnumerable<ulong>> GetAllSubscribedChannelsAsync(string id) => 
        await Repository.GetArrayFieldAsync(id, x => x.SubscribedChannels);

    public async Task AddSubscribedChannelAsync(string id, ulong channelId) => 
        await Repository.AddElementToArrayFieldAsync(id, x => x.SubscribedChannels, channelId);

    public async Task RemoveSubscribedChannelAsync(string id, ulong channelId) => 
        await Repository.RemoveElementFromArrayFieldAsync(id, x => x.SubscribedChannels, channelId);

    public async Task<IEnumerable<string>> GetAllSubscribedCrontabRulesAsync(string id) => 
        await Repository.GetArrayFieldAsync(id, x => x.SubscribedCrontabRules);

    public async Task AddCrontabRuleToGuildAsync(string id, string crontabRuleId) => 
        await Repository.AddElementToArrayFieldAsync(id, x => x.SubscribedCrontabRules, crontabRuleId);

    public async Task RemoveCrontabRuleFromGuildAsync(string id, string crontabRuleId) => 
        await Repository.RemoveElementFromArrayFieldAsync(id, x => x.SubscribedCrontabRules, crontabRuleId);

    public async Task<IEnumerable<string>> GetAllSubscribedResponseRulesAsync(string id) => 
        await Repository.GetArrayFieldAsync(id, x => x.SubscribedResponseRules);

    public async Task AddResponseRuleToGuildAsync(string id, string responseRuleId) => 
        await Repository.AddElementToArrayFieldAsync(id, x => x.SubscribedResponseRules, responseRuleId);

    public async Task RemoveResponseRuleFromGuildAsync(string id, string responseRuleId) => 
        await Repository.RemoveElementFromArrayFieldAsync(id, x => x.SubscribedResponseRules, responseRuleId);

    public async Task<IEnumerable<string>> GetAllSubscribedReactionRulesAsync(string id) => 
        await Repository.GetArrayFieldAsync(id, x => x.SubscribedReactionRules);

    public async Task AddReactionRuleToGuildAsync(string id, string reactionRuleId) => 
        await Repository.AddElementToArrayFieldAsync(id, x => x.SubscribedReactionRules, reactionRuleId);

    public async Task RemoveReactionRuleFromGuildAsync(string id, string reactionRuleId) => 
        await Repository.RemoveElementFromArrayFieldAsync(id, x => x.SubscribedReactionRules, reactionRuleId);
}