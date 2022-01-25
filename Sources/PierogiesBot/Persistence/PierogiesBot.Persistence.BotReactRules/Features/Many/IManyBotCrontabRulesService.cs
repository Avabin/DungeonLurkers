using PierogiesBot.Shared.Features.BotReactRules;
using Shared.Persistence.Core.Features.Documents.Many;

namespace PierogiesBot.Persistence.BotReactRules.Features.Many;

public interface IManyBotReactRulesService : IManyDocumentsService<BotReactRuleDocument, string, BotReactRuleDto>
{
}