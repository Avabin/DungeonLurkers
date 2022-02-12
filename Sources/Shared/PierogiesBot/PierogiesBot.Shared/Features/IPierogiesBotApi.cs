using PierogiesBot.Shared.Features.BotCrontabRules;
using PierogiesBot.Shared.Features.BotMessageSubscriptions;
using PierogiesBot.Shared.Features.BotReactRules;
using PierogiesBot.Shared.Features.BotResponseRules;

namespace PierogiesBot.Shared.Features;

public interface IPierogiesBotApi : IBotCrontabRuleApi, IBotMessageSubscriptionApi, IBotReactionRuleApi, IBotResponseRuleApi
{
    
}