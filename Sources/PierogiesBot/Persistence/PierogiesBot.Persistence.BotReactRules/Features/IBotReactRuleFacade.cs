using PierogiesBot.Shared.Features.BotReactRules;
using Shared.Persistence.Core.Features.Documents.Many;

namespace PierogiesBot.Persistence.BotReactRules.Features;

public interface IBotReactRuleFacade : IDocumentFacade<BotReactionRuleDocument, string, BotReactionRuleDto>
{
    Task AddReactionToRuleAsync(string      id, string response);
    Task RemoveReactionFromRuleAsync(string id, string response);
}