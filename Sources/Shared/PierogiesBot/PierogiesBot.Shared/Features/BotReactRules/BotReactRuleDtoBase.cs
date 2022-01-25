using PierogiesBot.Shared.Features.Dtos;

namespace PierogiesBot.Shared.Features.BotReactRules
{
    public abstract class BotReactRuleDtoBase : IBotSubscriptionRule
    {
        public abstract bool IsTriggerTextRegex { get; set; }
        public abstract bool ShouldTriggerOnContains { get; set; }
        public abstract string TriggerText { get; set; }
        public abstract StringComparison StringComparison { get; set; }
    }
}