using PierogiesBot.Persistence.BotCrontabRule.Features.Many;
using PierogiesBot.Persistence.BotCrontabRule.Features.Single;
using PierogiesBot.Shared.Features.BotCrontabRules;
using Shared.Persistence.Core.Features.Documents.Many;

namespace PierogiesBot.Persistence.BotCrontabRule.Features;

internal class BotCrontabRuleFacade : DocumentOperationFacade<BotCrontabRuleDocument, string, BotCrontabRuleDto>, IBotCrontabRuleFacade
{
    public BotCrontabRuleFacade(
        ISingleBotCrontabRuleService singleSingleDocumentService,
        IManyBotCrontabRulesService  manyManyDocumentsService) :
        base(singleSingleDocumentService, manyManyDocumentsService)
    {
    }

}