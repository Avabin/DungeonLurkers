using PierogiesBot.Shared.Features.BotCrontabRules;
using Shared.Persistence.Core.Features.Documents.Many;

namespace PierogiesBot.Persistence.BotCrontabRule.Features.Many;

public interface IManyBotCrontabRulesService : IManyDocumentsService<BotCrontabRuleDocument, string, BotCrontabRuleDto>
{
}