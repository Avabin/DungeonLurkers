using PierogiesBot.Persistence.BotResponseRules.Features.Many;
using PierogiesBot.Persistence.BotResponseRules.Features.Single;
using PierogiesBot.Shared.Features.BotResponseRules;
using Shared.Persistence.Core.Features.Documents.Many;

namespace PierogiesBot.Persistence.BotResponseRules.Features;

public class BotResponseRuleFacade : DocumentOperationFacade<BotResponseRuleDocument, string, BotResponseRuleDto>, IBotResponseRuleFacade
{
    private readonly ISingleBotResponseRuleService _singleSingleDocumentService;

    public BotResponseRuleFacade(
        ISingleBotResponseRuleService singleSingleDocumentService,
        IManyBotResponseRulesService  manyManyDocumentsService) :
        base(singleSingleDocumentService, manyManyDocumentsService)
    {
        _singleSingleDocumentService     = singleSingleDocumentService;
    }

    public Task RemoveResponseFromRuleAsync(string id, string response) => 
        _singleSingleDocumentService.RemoveResponseFromRuleAsync(id, response);

    public Task AddResponseToRuleAsync(string id, string response) => 
        _singleSingleDocumentService.AddResponseToRuleAsync(id, response);
}