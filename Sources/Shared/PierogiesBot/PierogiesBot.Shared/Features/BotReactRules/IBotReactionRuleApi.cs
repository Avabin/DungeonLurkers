using RestEase;
using Shared.Features.Authentication;

namespace PierogiesBot.Shared.Features.BotReactRules;

public interface IBotReactionRuleApi : IAuthenticatedApi
{
    [Get("BotReactRule")]
    Task<IEnumerable<BotReactionRuleDto>> GetAllReactionRulesAsync([Query] int? skip = null, [Query] int? limit = null);

    [Get("BotReactRule/{id}")] Task<BotReactionRuleDto> FindReactionRuleByIdAsync([Path] string id);

    [Post("BotReactRule")] Task<BotReactionRuleDto> CreateBotReactionRuleAsync([Body] CreateBotReactionRuleDto createDto);

    [Put("BotReactRule/{id}")]
    Task UpdateBotReactRuleAsync([Path] string id, [Body] UpdateBotReactionRuleDto updateDto);
    
    [Put("BotReactRule/{id}/Reactions")] Task AddReactionToRuleAsync([Path] string id, [Query] string reaction);
    [Delete("BotReactRule/{id}/Reactions")] Task RemoveReactionFromRuleAsync([Path] string id, [Query] string reaction);
    

    [Delete("BotReactRule/{id}")] Task DeleteBotReactionRuleAsync([Path] string id);
}