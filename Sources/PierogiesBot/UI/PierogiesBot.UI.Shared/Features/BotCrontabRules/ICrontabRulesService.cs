using PierogiesBot.Shared.Features.BotCrontabRules;

namespace PierogiesBot.UI.Shared.Features.BotCrontabRules;

public interface ICrontabRulesService
{
    Task<IEnumerable<BotCrontabRuleDto>> GetAllAsync(int?                        skip = null, int?   limit = null);
    Task                                 AddResponseToRuleAsync(string           ruleId,      string response);
    Task                                 RemoveResponseFromRuleAsync(string      ruleId,      string response);
    Task                                 AddEmoteToRuleAsync(string              ruleId,      string emote);
    Task                                 RemoveEmoteFromRuleAsync(string         ruleId,      string emote);
    Task                                 DeleteRuleAsync(string                  ruleId);
    Task<BotCrontabRuleDto>                                 CreateRuleAsync(CreateBotCrontabRuleDto request);
}