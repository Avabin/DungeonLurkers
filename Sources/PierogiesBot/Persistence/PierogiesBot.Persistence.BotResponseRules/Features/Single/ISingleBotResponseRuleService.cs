using PierogiesBot.Shared.Features.BotResponseRules;
using Shared.Persistence.Core.Features.Documents.Single;

namespace PierogiesBot.Persistence.BotResponseRules.Features.Single;

public interface ISingleBotResponseRuleService : ISingleDocumentService<BotResponseRuleDocument, string, BotResponseRuleDto>
{
    Task RemoveResponseFromRuleAsync(string id, string response);
    Task AddResponseToRuleAsync(string      id, string response);
}