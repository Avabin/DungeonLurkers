using PierogiesBot.Persistence.BotResponseRules.Features.Many;
using PierogiesBot.Persistence.BotResponseRules.Features.Single;
using PierogiesBot.Shared.Features.BotResponseRules;
using Shared.Persistence.Core.Features.Documents.Many;

namespace PierogiesBot.Persistence.BotResponseRules.Features;

public class BotResponseRuleFacade : DocumentFacade<BotResponseRuleDocument, string, BotResponseRuleDto>, IBotResponseRuleFacade
{
    private readonly ISingleBotResponseRuleService _singleDocumentService;

    public BotResponseRuleFacade(
        ISingleBotResponseRuleService singleDocumentService,
        IManyBotResponseRulesService  manyDocumentsService) :
        base(singleDocumentService, manyDocumentsService)
    {
        _singleDocumentService     = singleDocumentService;
    }

    public Task RemoveResponseFromRuleAsync(string id, string response) => 
        _singleDocumentService.RemoveResponseFromRuleAsync(id, response);

    public Task AddResponseToRuleAsync(string id, string response) => 
        _singleDocumentService.AddResponseToRuleAsync(id, response);

    public Task<IEnumerable<string>> GetResponsesForRuleAsync(string ruleId) => 
        _singleDocumentService.GetResponsesForRuleAsync(ruleId);
}