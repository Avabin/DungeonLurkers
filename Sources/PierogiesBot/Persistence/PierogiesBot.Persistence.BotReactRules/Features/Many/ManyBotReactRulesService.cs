using AutoMapper;
using PierogiesBot.Shared.Features.BotReactRules;
using Shared.Persistence.Core.Features.Documents.Many;
using Shared.Persistence.Core.Features.Repository;

namespace PierogiesBot.Persistence.BotReactRules.Features.Many;

public class ManyBotReactRulesService
    : ManyDocumentsService<BotReactionRuleDocument, string, BotReactionRuleDto>, IManyBotReactRulesService
{
    public ManyBotReactRulesService(IRepository<BotReactionRuleDocument, string> repository, IMapper mapper) : base(repository,
        mapper)
    {
    }
}