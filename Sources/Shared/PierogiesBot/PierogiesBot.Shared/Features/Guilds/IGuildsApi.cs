using RestEase;
using Shared.Features.Authentication;

namespace PierogiesBot.Shared.Features.Guilds;

public interface IGuildsApi : IAuthenticatedApi
{
    [Get("{PathPrefix}/Guild")]
    Task<IEnumerable<GuildDto>> GetGuildsAsync([Query] int? skip = null, [Query] int? limit = null);

    [Get("{PathPrefix}/Guild/{guildId}")] Task<GuildDto> GetGuildAsync([Path]    string          guildId);
    [Post("{PathPrefix}/Guild")]          Task<GuildDto> CreateGuildAsync([Body] CreateGuildDto createGuildDto);

    [Put("{PathPrefix}/Guild/{guildId}")]
    Task<GuildDto> UpdateGuildAsync([Path] string guildId, [Body] UpdateGuildDto updateGuildDto);

    [Delete("{PathPrefix}/Guild/{guildId}")]
    Task DeleteGuildAsync([Path] string guildId);

    [Get("{PathPrefix}/Guild/{guildId}/channels")]
    Task<GuildSubscribedChannelsDto> GetGuildSubscribedChannelsAsync([Path] string guildId);

    [Put("{PathPrefix}/Guild/{guildId}/channels")]
    Task AddGuildSubscribedChannelsAsync([Path] string guildId, [Query] ulong channelId);

    [Delete("{PathPrefix}/Guild/{guildId}/channels")]
    Task RemoveGuildSubscribedChannelsAsync([Path] string guildId, [Query] ulong channelId);

    [Get("{PathPrefix}/Guild/{guildId}/crontab")]
    Task<GuildSubscribedRulesDto> GetGuildSubscribedCrontabRulesAsync([Path] string guildId);

    [Put("{PathPrefix}/Guild/{guildId}/crontab")]
    Task AddGuildSubscribedCrontabRulesAsync([Path] string guildId, [Query] string ruleId);

    [Delete("{PathPrefix}/Guild/{guildId}/crontab")]
    Task RemoveGuildSubscribedCrontabRulesAsync([Path] string guildId, [Query] string ruleId);

    [Get("{PathPrefix}/Guild/{guildId}/reactions")]
    Task<GuildSubscribedRulesDto> GetGuildSubscribedReactionRulesAsync([Path] string guildId);

    [Put("{PathPrefix}/Guild/{guildId}/reactions")]
    Task AddGuildSubscribedReactionRulesAsync([Path] string guildId, [Query] string ruleId);

    [Delete("{PathPrefix}/Guild/{guildId}/reactions")]
    Task RemoveGuildSubscribedReactionRulesAsync([Path] string guildId, [Query] string ruleId);

    [Get("{PathPrefix}/Guild/{guildId}/responses")]
    Task<GuildSubscribedRulesDto> GetGuildSubscribedResponseRulesAsync([Path] string guildId);

    [Put("{PathPrefix}/Guild/{guildId}/responses")]
    Task AddGuildSubscribedResponseRulesAsync([Path] string guildId, [Query] string ruleId);

    [Delete("{PathPrefix}/Guild/{guildId}/responses")]
    Task RemoveGuildSubscribedResponseRulesAsync([Path] string guildId, [Query] string ruleId);
}