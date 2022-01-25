using AutoMapper;
using PierogiesBot.Shared.Features.BotReactRules;
using Shared.Persistence.Core.Features.Documents.Single;
using Shared.Persistence.Core.Features.Repository;

namespace PierogiesBot.Persistence.BotReactRules.Features.Single;

public class SingleBotReactRuleService
    : SingleDocumentService<BotReactRuleDocument, string, BotReactRuleDto>, ISingleBotReactRuleService
{
    public SingleBotReactRuleService(IRepository<BotReactRuleDocument, string> repository, IMapper mapper) : base(repository,
        mapper)
    {
    }
}