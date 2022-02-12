using AutoMapper;
using PierogiesBot.Shared.Features.BotReactRules;
using Shared.Persistence.Core.Features.Documents.Single;
using Shared.Persistence.Core.Features.Repository;

namespace PierogiesBot.Persistence.BotReactRules.Features.Single;

public class SingleBotReactRuleService
    : SingleDocumentService<BotReactionRuleDocument, string, BotReactionRuleDto>, ISingleBotReactRuleService
{
    public SingleBotReactRuleService(IRepository<BotReactionRuleDocument, string> repository, IMapper mapper) : base(repository,
        mapper)
    {
    }

    public async Task AddReactionToRuleAsync(string id, string reaction) => 
        await Repository.AddElementToArrayFieldAsync(id, x => x.Reactions, reaction);

    public async Task RemoveReactionFromRuleAsync(string id, string reaction) => 
        await Repository.RemoveElementFromArrayFieldAsync(id, x => x.Reactions, reaction);
}