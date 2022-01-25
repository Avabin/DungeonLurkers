using AutoMapper;
using PierogiesBot.Shared.Features.BotCrontabRules;
using Shared.Persistence.Core.Features.Documents.Many;
using Shared.Persistence.Core.Features.Repository;

namespace PierogiesBot.Persistence.BotCrontabRule.Features.Many;

internal class ManyBotCrontabRulesService
    : ManyDocumentsService<BotCrontabRuleDocument, string, BotCrontabRuleDto>, IManyBotCrontabRulesService
{
    public ManyBotCrontabRulesService(IRepository<BotCrontabRuleDocument, string> repository, IMapper mapper) : base(repository,
        mapper)
    {
    }
}