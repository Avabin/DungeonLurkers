using PierogiesBot.Shared.Features.BotResponseRules;
using Shared.Persistence.Core.Features.Documents.Many;

namespace PierogiesBot.Persistence.BotResponseRules.Features.Many;

public interface IManyBotResponseRulesService : IManyDocumentsService<BotResponseRuleDocument, string, BotResponseRuleDto>
{
}