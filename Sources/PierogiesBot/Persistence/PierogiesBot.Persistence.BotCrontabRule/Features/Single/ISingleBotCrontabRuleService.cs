using PierogiesBot.Shared.Features.BotCrontabRules;
using Shared.Persistence.Core.Features.Documents.Single;

namespace PierogiesBot.Persistence.BotCrontabRule.Features.Single;

public interface ISingleBotCrontabRuleService : ISingleDocumentService<BotCrontabRuleDocument, string, BotCrontabRuleDto>
{
}