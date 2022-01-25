using MongoDB.Bson;
using PierogiesBot.Persistence.Shared;
using PierogiesBot.Shared.Enums;

namespace PierogiesBot.Persistence.BotResponseRules.Features
{
    public record BotResponseRuleDocument(string Id, ResponseMode ResponseMode, IEnumerable<string> Responses,
        string TriggerText, StringComparison StringComparison, bool IsTriggerTextRegex, bool ShouldTriggerOnContains) :
        BotMessageRuleBase(Id, TriggerText, StringComparison, IsTriggerTextRegex, ShouldTriggerOnContains)
    {
        public BotResponseRuleDocument(ResponseMode responseMode, IEnumerable<string> responses, string triggerText,
            StringComparison stringComparison, bool isTriggerTextRegex, bool shouldTriggerOnContains)
            : this(ObjectId.GenerateNewId().ToString(), responseMode, responses, triggerText, stringComparison,
                isTriggerTextRegex, shouldTriggerOnContains)
        {
        }
    }
}