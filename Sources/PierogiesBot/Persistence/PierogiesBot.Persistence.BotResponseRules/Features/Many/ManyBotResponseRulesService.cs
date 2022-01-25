using AutoMapper;
using PierogiesBot.Shared.Features.BotResponseRules;
using Shared.Persistence.Core.Features.Documents.Many;
using Shared.Persistence.Core.Features.Repository;

namespace PierogiesBot.Persistence.BotResponseRules.Features.Many;

public class ManyBotResponseRulesService
    : ManyDocumentsService<BotResponseRuleDocument, string, BotResponseRuleDto>, IManyBotResponseRulesService
{
    public ManyBotResponseRulesService(IRepository<BotResponseRuleDocument, string> repository, IMapper mapper) : base(repository,
        mapper)
    {
    }
}