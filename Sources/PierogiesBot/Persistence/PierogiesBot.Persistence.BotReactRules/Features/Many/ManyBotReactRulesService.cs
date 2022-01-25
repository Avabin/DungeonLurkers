using AutoMapper;
using PierogiesBot.Shared.Features.BotReactRules;
using Shared.Persistence.Core.Features.Documents.Many;
using Shared.Persistence.Core.Features.Repository;

namespace PierogiesBot.Persistence.BotReactRules.Features.Many;

public class ManyBotReactRulesService
    : ManyDocumentsService<BotReactRuleDocument, string, BotReactRuleDto>, IManyBotReactRulesService
{
    public ManyBotReactRulesService(IRepository<BotReactRuleDocument, string> repository, IMapper mapper) : base(repository,
        mapper)
    {
    }
}