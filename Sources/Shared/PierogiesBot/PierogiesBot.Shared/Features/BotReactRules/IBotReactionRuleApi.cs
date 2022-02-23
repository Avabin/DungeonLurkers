using RestEase;
using Shared.Features.Authentication;

namespace PierogiesBot.Shared.Features.BotReactRules;

public interface IBotReactionRuleApi : IAuthenticatedApi
{
    [Get("{PathPrefix}/BotReactRule")]
    Task<IEnumerable<BotReactionRuleDto>> GetAllReactionRulesAsync([Query] int? skip = null, [Query] int? limit = null);

    [Get("{PathPrefix}/BotReactRule/{id}")] Task<BotReactionRuleDto> FindReactionRuleByIdAsync([Path] string id);

    [Post("{PathPrefix}/BotReactRule")] Task<BotReactionRuleDto> CreateBotReactionRuleAsync([Body] CreateBotReactionRuleDto createDto);

    [Put("{PathPrefix}/BotReactRule/{id}")]
    Task UpdateBotReactRuleAsync([Path] string id, [Body] UpdateBotReactionRuleDto updateDto);
    
    [Put("{PathPrefix}/BotReactRule/{id}/Reactions")] Task AddReactionToRuleAsync([Path] string id, [Query] string reaction);
    [Delete("{PathPrefix}/BotReactRule/{id}/Reactions")] Task RemoveReactionFromRuleAsync([Path] string id, [Query] string reaction);
    

    [Delete("{PathPrefix}/BotReactRule/{id}")] Task DeleteBotReactionRuleAsync([Path] string id);
}