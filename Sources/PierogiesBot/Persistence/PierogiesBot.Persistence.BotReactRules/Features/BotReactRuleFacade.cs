using PierogiesBot.Persistence.BotReactRules.Features.Many;
using PierogiesBot.Persistence.BotReactRules.Features.Single;
using PierogiesBot.Shared.Features.BotReactRules;
using Shared.Persistence.Core.Features.Documents.Many;

namespace PierogiesBot.Persistence.BotReactRules.Features;

public class BotReactRuleFacade : DocumentFacade<BotReactionRuleDocument, string, BotReactionRuleDto>, IBotReactRuleFacade
{
    private readonly ISingleBotReactRuleService _singleDocumentService;

    public BotReactRuleFacade(
        ISingleBotReactRuleService singleDocumentService,
        IManyBotReactRulesService  manyDocumentsService) :
        base(singleDocumentService, manyDocumentsService)
    {
        _singleDocumentService = singleDocumentService;
    }

    public Task AddReactionToRuleAsync(string id, string response) => 
        _singleDocumentService.AddReactionToRuleAsync(id, response);

    public Task RemoveReactionFromRuleAsync(string id, string response) => 
        _singleDocumentService.RemoveReactionFromRuleAsync(id, response);
}