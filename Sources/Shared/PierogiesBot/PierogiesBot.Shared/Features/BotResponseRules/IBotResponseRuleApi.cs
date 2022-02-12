using RestEase;
using Shared.Features.Authentication;

namespace PierogiesBot.Shared.Features.BotResponseRules;

public interface IBotResponseRuleApi : IAuthenticatedApi
{
    [Get("BotResponseRule")]
    Task<IEnumerable<BotResponseRuleDto>> GetAllResponseRulesAsync([Query] int? skip = null, [Query] int? limit = null);

    [Get("BotResponseRule/{id}")] Task<BotResponseRuleDto> FindResponseRuleByIdAsync([Path] string id);

    [Post("BotResponseRule")] Task<BotResponseRuleDto> CreateBotResponseRuleAsync([Body] CreateBotResponseRuleDto createDto);

    [Put("BotResponseRule/{id}")]
    Task UpdateBotResponseRuleAsync([Path] string id, [Body] UpdateBotResponseRuleDto updateDto);
    
    [Post("BotResponseRule/{id}/responses")]
    Task AddResponseToRule([Path] string id, [Query] string response);
    
    [Delete("BotResponseRule/{id}/responses")]
    Task RemoveResponseFromRule([Path] string id, [Query] string response);

    [Delete("BotResponseRule/{id}")] Task DeleteBotResponseRuleAsync([Path] string id);
}