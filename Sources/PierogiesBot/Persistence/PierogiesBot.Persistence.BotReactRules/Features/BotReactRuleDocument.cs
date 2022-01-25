using MongoDB.Bson;
using PierogiesBot.Persistence.Shared;
using PierogiesBot.Shared.Enums;

namespace PierogiesBot.Persistence.BotReactRules.Features
{
    public record BotReactRuleDocument(string Id, IEnumerable<string> Reactions, string TriggerText,
        StringComparison StringComparison, bool IsTriggerTextRegex, bool ShouldTriggerOnContains,
        ResponseMode ResponseMode = ResponseMode.First) : BotMessageRuleBase(Id, TriggerText, StringComparison,
        IsTriggerTextRegex,
        ShouldTriggerOnContains)

    {
        public BotReactRuleDocument(IEnumerable<string> reactions, string triggerText, StringComparison stringComparison,
            bool isTriggerTextRegex, bool shouldTriggerOnContains, ResponseMode responseMode = ResponseMode.First)
            : this(ObjectId.GenerateNewId().ToString(), reactions, triggerText, stringComparison, isTriggerTextRegex,
                shouldTriggerOnContains, responseMode)
        {
        }
    }
}