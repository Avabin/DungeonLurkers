using PierogiesBot.Persistence.BotReactRules.Features.Many;
using PierogiesBot.Persistence.BotReactRules.Features.Single;
using PierogiesBot.Shared.Features.BotReactRules;
using Shared.Persistence.Core.Features.Documents.Many;

namespace PierogiesBot.Persistence.BotReactRules.Features;

public class BotReactRuleFacade : DocumentOperationFacade<BotReactionRuleDocument, string, BotReactionRuleDto>, IBotReactRuleFacade
{
    private readonly ISingleBotReactRuleService _singleSingleDocumentService;

    public BotReactRuleFacade(
        ISingleBotReactRuleService singleSingleDocumentService,
        IManyBotReactRulesService  manyManyDocumentsService) :
        base(singleSingleDocumentService, manyManyDocumentsService)
    {
        _singleSingleDocumentService = singleSingleDocumentService;
    }

    public Task AddReactionToRuleAsync(string id, string response) => 
        _singleSingleDocumentService.AddReactionToRuleAsync(id, response);

    public Task RemoveReactionFromRuleAsync(string id, string response) => 
        _singleSingleDocumentService.RemoveReactionFromRuleAsync(id, response);
}