using RestEase;
using Shared.Features.Authentication;

namespace PierogiesBot.Shared.Features.BotCrontabRules;

public interface IBotCrontabRuleApi : IAuthenticatedApi
{
    [Get("{PathPrefix}/BotCrontabRule")]
    Task<IEnumerable<BotCrontabRuleDto>> GetAllCrontabRulesAsync([Query] int? skip = null, [Query] int? limit = null);

    [Get("{PathPrefix}/BotCrontabRule/{id}")]
    Task<BotCrontabRuleDto> FindCrontabRuleByIdAsync([Path] string id);

    [Post("{PathPrefix}/BotCrontabRule")]
    Task<BotCrontabRuleDto> CreateBotCrontabRuleAsync([Body] CreateBotCrontabRuleDto createDto);

    [Post("{PathPrefix}/BotCrontabRule/{id}/responses")]
    Task<BotCrontabRuleDto> AddResponseToCrontabRuleAsync([Path] string id, [Query] string response);
    
    [Delete("{PathPrefix}/BotCrontabRule/{id}/responses")]
    Task DeleteResponseFromCrontabRuleAsync([Path] string id, [Query] string response);
    
    [Post("{PathPrefix}/BotCrontabRule/{id}/emotes")]
    Task<BotCrontabRuleDto> AddEmoteToCrontabRuleAsync([Path] string id, [Query] string emote);
    
    [Delete("{PathPrefix}/BotCrontabRule/{id}/emotes")]
    Task DeleteEmoteFromCrontabRuleAsync([Path] string id, [Query] string emote);

    [Put("{PathPrefix}/BotCrontabRule/{id}")]
    Task UpdateBotCrontabRuleAsync([Path] string id, [Body] UpdateBotCrontabRuleDto updateDto);

    [Delete("{PathPrefix}/BotCrontabRule/{id}")]
    Task DeleteBotCrontabRuleAsync([Path] string id);
}