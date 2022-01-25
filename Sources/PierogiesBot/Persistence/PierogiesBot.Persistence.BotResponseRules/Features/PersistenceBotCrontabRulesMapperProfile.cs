using PierogiesBot.Shared.Features.BotResponseRules;
using Shared.Features;

namespace PierogiesBot.Persistence.BotResponseRules.Features;

public class PersistenceBotResponseRulesMapperProfile
    : DtoMapperProfile<BotResponseRuleDto, CreateBotResponseRuleDto, UpdateBotResponseRuleDto, BotResponseRuleDocument>
{
}