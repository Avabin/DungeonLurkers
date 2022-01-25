using PierogiesBot.Shared.Features.BotReactRules;
using Shared.Persistence.Core.Features.Documents.Many;

namespace PierogiesBot.Persistence.BotReactRules.Features;

public interface IBotReactRuleFacade : IDocumentOperationFacade<BotReactRuleDocument, string, BotReactRuleDto>
{
}