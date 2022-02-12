using PierogiesBot.Persistence.BotCrontabRule.Features.Many;
using PierogiesBot.Persistence.BotCrontabRule.Features.Single;
using PierogiesBot.Shared.Features.BotCrontabRules;
using Shared.Persistence.Core.Features.Documents.Many;

namespace PierogiesBot.Persistence.BotCrontabRule.Features;

internal class BotCrontabRuleFacade : DocumentFacade<BotCrontabRuleDocument, string, BotCrontabRuleDto>, IBotCrontabRuleFacade
{
    public BotCrontabRuleFacade(
        ISingleBotCrontabRuleService singleDocumentService,
        IManyBotCrontabRulesService  manyDocumentsService) :
        base(singleDocumentService, manyDocumentsService)
    {
    }

}