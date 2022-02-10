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

    public async Task RemoveResponseFromRuleAsync(string id, string response) => 
        await Repository.RemoveElementFromArrayFieldAsync(id, x => x.Responses, response);
    
    public async Task AddResponseToRuleAsync(string id, string response) => 
        await Repository.AddElementToArrayFieldAsync(id, x => x.Responses, response);
}