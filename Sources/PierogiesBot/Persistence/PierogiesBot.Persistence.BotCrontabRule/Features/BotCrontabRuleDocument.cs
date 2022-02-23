using MongoDB.Bson;
using Newtonsoft.Json;
using PierogiesBot.Shared.Enums;
using Shared.Persistence.Core.Features.Documents;

namespace PierogiesBot.Persistence.BotCrontabRule.Features;

public record BotCrontabRuleDocument : DocumentBase<string>
{
    public BotCrontabRuleDocument(bool                isEmoji, string crontab,
                                  IEnumerable<string> replyMessages,
                                  IEnumerable<string> replyEmojis,
                                  ResponseMode        responseMode) :
        this(ObjectId.GenerateNewId().ToString(),
             isEmoji, crontab, replyMessages,
             replyEmojis, responseMode)
    {
    }

    [JsonConstructor]
    public BotCrontabRuleDocument(string              id, bool isEmoji, string crontab,
                                  IEnumerable<string> replyMessages,
                                  IEnumerable<string> replyEmojis,
                                  ResponseMode        responseMode) : base(id)
    {
        IsEmoji       = isEmoji;
        Crontab       = crontab;
        ReplyMessages = replyMessages;
        ReplyEmojis   = replyEmojis;
        ResponseMode  = responseMode;
    }

    public bool                IsEmoji       { get; init; }
    public string              Crontab       { get; init; }
    public IEnumerable<string> ReplyMessages { get; init; }
    public IEnumerable<string> ReplyEmojis   { get; init; }
    public ResponseMode        ResponseMode  { get; init; }

    public void Deconstruct(out string              id,            out bool                isEmoji, out string crontab,
                            out IEnumerable<string> replyMessages, out IEnumerable<string> replyEmojis,
                            out ResponseMode        responseMode)
    {
        id            = Id;
        isEmoji       = IsEmoji;
        crontab       = Crontab;
        replyMessages = ReplyMessages;
        replyEmojis   = ReplyEmojis;
        responseMode  = ResponseMode;
    }
}