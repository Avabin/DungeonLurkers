using PierogiesBot.Shared.Features.BotResponseRules;
using Shared.Persistence.Core.Features.Documents.Many;

namespace PierogiesBot.Persistence.BotResponseRules.Features;

public interface IBotResponseRuleFacade : IDocumentOperationFacade<BotResponseRuleDocument, string, BotResponseRuleDto>
{
}