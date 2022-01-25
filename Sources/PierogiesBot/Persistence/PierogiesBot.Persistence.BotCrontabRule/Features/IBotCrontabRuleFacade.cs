using PierogiesBot.Shared.Features.BotCrontabRules;
using Shared.Persistence.Core.Features.Documents.Many;

namespace PierogiesBot.Persistence.BotCrontabRule.Features;

public interface IBotCrontabRuleFacade : IDocumentOperationFacade<BotCrontabRuleDocument, string, BotCrontabRuleDto>
{
}