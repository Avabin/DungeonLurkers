using PierogiesBot.Shared.Features.BotReactRules;
using Shared.Persistence.Core.Features.Documents.Single;

namespace PierogiesBot.Persistence.BotReactRules.Features.Single;

public interface ISingleBotReactRuleService : ISingleDocumentService<BotReactRuleDocument, string, BotReactRuleDto>
{
}