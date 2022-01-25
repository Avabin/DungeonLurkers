using AutoMapper;
using PierogiesBot.Shared.Features.BotCrontabRules;
using Shared.Persistence.Core.Features.Documents.Single;
using Shared.Persistence.Core.Features.Repository;

namespace PierogiesBot.Persistence.BotCrontabRule.Features.Single;

internal class SingleBotCrontabRuleService
    : SingleDocumentService<BotCrontabRuleDocument, string, BotCrontabRuleDto>, ISingleBotCrontabRuleService
{
    public SingleBotCrontabRuleService(IRepository<BotCrontabRuleDocument, string> repository, IMapper mapper) : base(repository,
        mapper)
    {
    }
}