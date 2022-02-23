using PierogiesBot.Shared.Features.BotCrontabRules;
using Shared.Persistence.Core.Features.Documents.Single;

namespace PierogiesBot.Persistence.BotCrontabRule.Features.Single;

public interface ISingleBotCrontabRuleService : ISingleDocumentService<BotCrontabRuleDocument, string, BotCrontabRuleDto>
{
    Task<IEnumerable<string>> GetResponsesForRuleAsync(string    ruleId);
    Task                      AddResponseToRuleAsync(string      ruleId, string response);
    Task                      AddEmojiToRuleAsync(string         ruleId, string emoji);
    Task                      RemoveResponseFromRuleAsync(string ruleId, string response);
    Task                      RemoveEmojiFromRuleAsync(string ruleId, string emoji);
}