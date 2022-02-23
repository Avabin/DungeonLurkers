using PierogiesBot.Shared.Features.BotCrontabRules;
using Shared.Persistence.Core.Features.Documents.Many;

namespace PierogiesBot.Persistence.BotCrontabRule.Features;

public interface IBotCrontabRuleFacade : IDocumentFacade<BotCrontabRuleDocument, string, BotCrontabRuleDto>
{
    Task<IEnumerable<string>> GetResponsesForRuleAsync(string    ruleId);
    Task                      AddResponseToRuleAsync(string      ruleId, string response);
    Task                      AddEmojiToRuleAsync(string         ruleId, string emoji);
    Task                      RemoveResponseFromRuleAsync(string ruleId, string response);
    Task                      RemoveEmojiFromRuleAsync(string    id,     string response);
}