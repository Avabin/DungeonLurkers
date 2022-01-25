using AutoMapper;
using PierogiesBot.Shared.Features.BotResponseRules;
using Shared.Persistence.Core.Features.Documents.Single;
using Shared.Persistence.Core.Features.Repository;

namespace PierogiesBot.Persistence.BotResponseRules.Features.Single;

public class SingleBotResponseRuleService
    : SingleDocumentService<BotResponseRuleDocument, string, BotResponseRuleDto>, ISingleBotResponseRuleService
{
    public SingleBotResponseRuleService(IRepository<BotResponseRuleDocument, string> repository, IMapper mapper) : base(repository,
        mapper)
    {
    }
}