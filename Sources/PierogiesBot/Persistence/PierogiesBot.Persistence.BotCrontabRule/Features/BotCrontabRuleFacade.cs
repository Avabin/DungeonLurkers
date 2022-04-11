using PierogiesBot.Persistence.BotCrontabRule.Features.Many;
using PierogiesBot.Persistence.BotCrontabRule.Features.Single;
using PierogiesBot.Shared.Features.BotCrontabRules;
using Shared.Persistence.Core.Features.Documents.Many;

namespace PierogiesBot.Persistence.BotCrontabRule.Features;

internal class BotCrontabRuleFacade : DocumentFacade<BotCrontabRuleDocument, string, BotCrontabRuleDto>, IBotCrontabRuleFacade
{
    private readonly ISingleBotCrontabRuleService _singleDocumentService;

    public BotCrontabRuleFacade(
        ISingleBotCrontabRuleService singleDocumentService,
        IManyBotCrontabRulesService  manyDocumentsService) :
        base(singleDocumentService, manyDocumentsService)
    {
        _singleDocumentService = singleDocumentService;
    }

    public async Task<IEnumerable<string>> GetResponsesForRuleAsync(string ruleId) => 
        await _singleDocumentService.GetResponsesForRuleAsync(ruleId);

    public async Task AddResponseToRuleAsync(string ruleId, string response) => 
        await _singleDocumentService.AddResponseToRuleAsync(ruleId, response);

    public async Task AddEmojiToRuleAsync(string ruleId, string emoji) => 
        await _singleDocumentService.AddEmojiToRuleAsync(ruleId, emoji);

    public async Task RemoveResponseFromRuleAsync(string ruleId, string response) => 
        await _singleDocumentService.RemoveResponseFromRuleAsync(ruleId, response);
    public async Task RemoveEmojiFromRuleAsync(string    id,     string response) => 
        await _singleDocumentService.RemoveEmojiFromRuleAsync(id, response);
}