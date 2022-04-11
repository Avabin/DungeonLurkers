using PierogiesBot.Shared.Features.BotCrontabRules;
using PierogiesBot.Shared.Features.BotMessageSubscriptions;
using PierogiesBot.Shared.Features.BotReactRules;
using PierogiesBot.Shared.Features.BotResponseRules;
using RestEase;
using Shared.Features.Authentication;
using Shared.Features.Users;

namespace PierogiesBot.Shared.Features;

public interface IPierogiesBotApi : IBotCrontabRuleApi, IBotMessageSubscriptionApi, IBotReactionRuleApi, IBotResponseRuleApi, IUsersApi
{
}