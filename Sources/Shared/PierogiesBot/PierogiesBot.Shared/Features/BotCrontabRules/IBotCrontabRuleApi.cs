using RestEase;
using Shared.Features.Authentication;

namespace PierogiesBot.Shared.Features.BotCrontabRules;

public interface IBotCrontabRuleApi : IAuthenticatedApi
{
    [Get("BotCrontabRule")]
    Task<IEnumerable<BotCrontabRuleDto>> GetAllAsync([Query] int? skip = null, [Query] int? limit = null);

    [Get("BotCrontabRule/{id}")] Task<BotCrontabRuleDto> FindByIdAsync([Path] string id);

    [Post("BotCrontabRule")] Task<BotCrontabRuleDto> CreateBotCrontabRuleAsync([Body] CreateBotCrontabRuleDto createDto);

    [Put("BotCrontabRule/{id}")]
    Task UpdateBotCrontabRuleAsync([Path] string id, [Body] UpdateBotCrontabRuleDto updateDto);

    [Delete("BotCrontabRule/{id}")] Task DeleteBotCrontabRuleAsync([Path] string id);
}