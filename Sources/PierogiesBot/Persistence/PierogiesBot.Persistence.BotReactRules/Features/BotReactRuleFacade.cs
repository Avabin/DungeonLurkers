using PierogiesBot.Persistence.BotReactRules.Features.Many;
using PierogiesBot.Persistence.BotReactRules.Features.Single;
using PierogiesBot.Shared.Features.BotReactRules;
using Shared.Persistence.Core.Features.Documents.Many;

namespace PierogiesBot.Persistence.BotReactRules.Features;

public class BotReactRuleFacade : DocumentOperationFacade<BotReactRuleDocument, string, BotReactRuleDto>, IBotReactRuleFacade
{
    public BotReactRuleFacade(
        ISingleBotReactRuleService singleDocumentService,
        IManyBotReactRulesService  manyDocumentsService) :
        base(singleDocumentService, manyDocumentsService)
    {
    }
}