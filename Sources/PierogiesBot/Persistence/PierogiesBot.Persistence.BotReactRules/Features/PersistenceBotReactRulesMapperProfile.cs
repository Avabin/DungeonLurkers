using PierogiesBot.Shared.Features.BotReactRules;
using Shared.Features;

namespace PierogiesBot.Persistence.BotReactRules.Features;

public class PersistenceBotReactRulesMapperProfile
    : DtoMapperProfile<BotReactRuleDto, CreateBotReactRuleDto, UpdateBotReactRuleDto, BotReactRuleDocument>
{
}