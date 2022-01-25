using MongoDB.Bson;
using PierogiesBot.Shared.Enums;
using Shared.Persistence.Core.Features.Documents;

namespace PierogiesBot.Persistence.BotCrontabRule.Features
{
    public record BotCrontabRuleDocument(string Id, bool IsEmoji, string Crontab, IEnumerable<string> ReplyMessages,
                                         IEnumerable<string> ReplyEmojis,
                                         ResponseMode ResponseMode) : DocumentBase<string>(Id)
    {
        public BotCrontabRuleDocument(bool                isEmoji, string crontab, IEnumerable<string> replyMessages,
                                      IEnumerable<string> replyEmojis, ResponseMode responseMode) :
            this(ObjectId.GenerateNewId().ToString(),
                 isEmoji, crontab, replyMessages,
                 replyEmojis, responseMode)
        {
        }
    }
}