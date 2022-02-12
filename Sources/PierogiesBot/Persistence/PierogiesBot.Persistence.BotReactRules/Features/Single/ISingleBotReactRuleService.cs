using PierogiesBot.Shared.Features.BotReactRules;
using Shared.Persistence.Core.Features.Documents.Single;

namespace PierogiesBot.Persistence.BotReactRules.Features.Single;

public interface ISingleBotReactRuleService : ISingleDocumentService<BotReactionRuleDocument, string, BotReactionRuleDto>
{
    Task AddReactionToRuleAsync(string      id, string reaction);
    Task RemoveReactionFromRuleAsync(string id, string reaction);
}