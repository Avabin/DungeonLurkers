using PierogiesBot.Shared.Features.BotReactRules;
using Shared.Features;

namespace PierogiesBot.Persistence.BotReactRules.Features;

public class PersistenceBotReactRulesMapperProfile
    : DtoMapperProfile<BotReactionRuleDto, CreateBotReactionRuleDto, UpdateBotReactionRuleDto, BotReactionRuleDocument>
{
}