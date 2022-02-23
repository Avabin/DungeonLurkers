using PierogiesBot.Shared.Features.BotCrontabRules;

namespace PierogiesBot.UI.Shared.Features.BotCrontabRules;

public class CrontabRulesService : ICrontabRulesService
{
    private readonly IBotCrontabRuleApi _api;

    public CrontabRulesService(IBotCrontabRuleApi api) => 
        _api = api;

    public async Task<IEnumerable<BotCrontabRuleDto>> GetAllAsync(int? skip = null, int? limit = null)
    {
        var allCrontabRulesAsync = await _api.GetAllCrontabRulesAsync(skip, limit);
        return allCrontabRulesAsync;
    }

    public async Task AddResponseToRuleAsync(string ruleId, string response) => 
        await _api.AddResponseToCrontabRuleAsync(ruleId, response);

    public async Task RemoveResponseFromRuleAsync(string ruleId, string response) => 
        await _api.DeleteResponseFromCrontabRuleAsync(ruleId, response);

    public async Task AddEmoteToRuleAsync(string ruleId, string emote) => 
        await _api.AddEmoteToCrontabRuleAsync(ruleId, emote);

    public async Task RemoveEmoteFromRuleAsync(string ruleId, string emote) => 
        await _api.DeleteEmoteFromCrontabRuleAsync(ruleId, emote);

    public async Task DeleteRuleAsync(string ruleId) => 
        await _api.DeleteBotCrontabRuleAsync(ruleId);
}