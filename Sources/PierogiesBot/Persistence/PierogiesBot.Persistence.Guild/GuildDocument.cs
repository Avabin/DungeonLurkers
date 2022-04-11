using System.Reflection.Metadata;
using MongoDB.Bson;
using Newtonsoft.Json;
using Shared.Persistence.Core.Features.Documents;

namespace PierogiesBot.Persistence.Guild;

public record GuildDocument : DocumentBase<string>
{
    public GuildDocument(string       name,
                         string       iconUri,
                         ulong discordId,
                         string timezoneId,
                         List<ulong> subscribedChannels,
                         List<string> subscribedCrontabRules,
                         List<string> subscribedResponseRules,
                         List<string> subscribedReactionRules)
        : this(ObjectId.GenerateNewId().ToString(), discordId,name, iconUri, timezoneId, subscribedChannels, subscribedCrontabRules,
               subscribedResponseRules, subscribedReactionRules)
    {
    }

    [JsonConstructor]
    public GuildDocument(string       id,
                         ulong        discordId,
                         string       name,
                         string       iconUri,
                         string       timezoneId,
                         List<ulong>  subscribedChannels,
                         List<string> subscribedCrontabRules,
                         List<string> subscribedResponseRules,
                         List<string> subscribedReactionRules) : base(id)
    {
        Id = id;
        DiscordId               = discordId;
        Name                    = name;
        IconUri                 = iconUri;
        TimezoneId              = timezoneId;
        SubscribedChannels      = subscribedChannels;
        SubscribedCrontabRules  = subscribedCrontabRules;
        SubscribedResponseRules = subscribedResponseRules;
        SubscribedReactionRules = subscribedReactionRules;
    }

    public ulong        DiscordId               { get; init; }
    public string       Name                    { get; init; }
    public string       IconUri                 { get; init; }
    public string       TimezoneId              { get; init; }
    public List<ulong>  SubscribedChannels      { get; init; }
    public List<string> SubscribedCrontabRules  { get; init; }
    public List<string> SubscribedResponseRules { get; init; }
    public List<string> SubscribedReactionRules { get; init; }
}