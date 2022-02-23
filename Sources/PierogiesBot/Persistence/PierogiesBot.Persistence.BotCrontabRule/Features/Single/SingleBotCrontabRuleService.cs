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

    public async Task<IEnumerable<string>> GetResponsesForRuleAsync(string ruleId)                  => 
        await Repository.GetArrayFieldAsync(ruleId, x => x.ReplyMessages);
    public async Task                      AddResponseToRuleAsync(string   ruleId, string response) => 
        await Repository.AddElementToArrayFieldAsync(ruleId, x => x.ReplyMessages, response);
    public async Task                      AddEmojiToRuleAsync(string      ruleId, string emoji)    => 
        await Repository.AddElementToArrayFieldAsync(ruleId, x => x.ReplyEmojis,        emoji);

    public async Task RemoveResponseFromRuleAsync(string ruleId, string response) => 
        await Repository.RemoveElementFromArrayFieldAsync(ruleId, x => x.ReplyMessages, response);

    public async Task RemoveEmojiFromRuleAsync(string ruleId, string emoji) => 
        await Repository.RemoveElementFromArrayFieldAsync(ruleId, x => x.ReplyEmojis, emoji);

    public async Task<IEnumerable<string>> GetEmojisForRuleAsync(string ruleId) => 
        await Repository.GetArrayFieldAsync(ruleId, x => x.ReplyEmojis);
}