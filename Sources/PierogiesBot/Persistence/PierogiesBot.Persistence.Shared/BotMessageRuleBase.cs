using MongoDB.Bson;
using Shared.Persistence.Core.Features.Documents;

namespace PierogiesBot.Persistence.Shared
{
    public abstract record BotMessageRuleBase(string Id, string TriggerText, StringComparison StringComparison,
        bool IsTriggerTextRegex, bool ShouldTriggerOnContains) : DocumentBase<string>(Id)
    {
        protected BotMessageRuleBase(string triggerText, StringComparison stringComparison, bool isTriggerTextRegex,
            bool shouldTriggerOnContains)
            : this(ObjectId.GenerateNewId().ToString(), triggerText, stringComparison, isTriggerTextRegex,
                shouldTriggerOnContains)
        {
        }
    }
}