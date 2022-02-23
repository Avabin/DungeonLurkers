using RestEase;
using Shared.Features.Authentication;

namespace PierogiesBot.Shared.Features.BotResponseRules;

public interface IBotResponseRuleApi : IAuthenticatedApi
{
    [Get("{PathPrefix}/BotResponseRule")]
    Task<IEnumerable<BotResponseRuleDto>> GetAllResponseRulesAsync([Query] int? skip = null, [Query] int? limit = null);

    [Get("{PathPrefix}/BotResponseRule/{id}")] Task<BotResponseRuleDto> FindResponseRuleByIdAsync([Path] string id);

    [Post("{PathPrefix}/BotResponseRule")] Task<BotResponseRuleDto> CreateBotResponseRuleAsync([Body] CreateBotResponseRuleDto createDto);

    [Put("{PathPrefix}/BotResponseRule/{id}")]
    Task UpdateBotResponseRuleAsync([Path] string id, [Body] UpdateBotResponseRuleDto updateDto);
    
    [Post("{PathPrefix}/BotResponseRule/{id}/responses")]
    Task AddResponseToResponseRule([Path] string id, [Query] string response);
    
    [Delete("{PathPrefix}/BotResponseRule/{id}/responses")]
    Task RemoveResponseFromResponseRule([Path] string id, [Query] string response);

    [Delete("{PathPrefix}/BotResponseRule/{id}")] Task DeleteBotResponseRuleAsync([Path] string id);
}