using PierogiesBot.Persistence.BotResponseRules.Features.Many;
using PierogiesBot.Persistence.BotResponseRules.Features.Single;
using PierogiesBot.Shared.Features.BotResponseRules;
using Shared.Persistence.Core.Features.Documents.Many;

namespace PierogiesBot.Persistence.BotResponseRules.Features;

public class BotResponseRuleFacade : DocumentOperationFacade<BotResponseRuleDocument, string, BotResponseRuleDto>, IBotResponseRuleFacade
{
    public BotResponseRuleFacade(
        ISingleBotResponseRuleService singleDocumentService,
        IManyBotResponseRulesService  manyDocumentsService) :
        base(singleDocumentService, manyDocumentsService)
    {
    }
}