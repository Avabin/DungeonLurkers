using PierogiesBot.Shared.Features.BotCrontabRules;
using Shared.Features;

namespace PierogiesBot.Persistence.BotCrontabRule.Features;

public class PersistenceBotCrontabRulesMapperProfile
    : DtoMapperProfile<BotCrontabRuleDto, CreateBotCrontabRuleDto, UpdateBotCrontabRuleDto, BotCrontabRuleDocument>
{
}