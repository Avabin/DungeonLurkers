using PierogiesBot.Shared.Features.BotMessageSubscriptions;
using RestEase;
using Shared.Features.Authentication;

namespace PierogiesBot.Shared.Features.BotReactRules;

public interface IBotReactRuleApi : IAuthenticatedApi
{
    [Get("BotReactRule")]
    Task<IEnumerable<BotReactRuleDto>> GetAllAsync([Query] int? skip = null, [Query] int? limit = null);

    [Get("BotReactRule/{id}")] Task<BotReactRuleDto> FindByIdAsync([Path] string id);

    [Post("BotReactRule")] Task<BotReactRuleDto> CreateBotReactRuleAsync([Body] CreateBotReactRuleDto createDto);

    [Put("BotReactRule/{id}")]
    Task UpdateBotReactRuleAsync([Path] string id, [Body] UpdateBotReactRuleDto updateDto);

    [Delete("BotReactRule/{id}")] Task DeleteBotReactRuleAsync([Path] string id);
}