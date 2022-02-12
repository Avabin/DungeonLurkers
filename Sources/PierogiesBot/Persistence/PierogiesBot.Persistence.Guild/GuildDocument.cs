using System.Reflection.Metadata;
using MongoDB.Bson;
using Newtonsoft.Json;
using Shared.Persistence.Core.Features.Documents;

namespace PierogiesBot.Persistence.Guild;

public record GuildDocument : DocumentBase<string>
{
    public GuildDocument(ulong discordId, string timezoneId, List<ulong> subscribedChannels)
        : this(ObjectId.GenerateNewId().ToString(), discordId, timezoneId, subscribedChannels)
    {
    }

    [JsonConstructor]
    public GuildDocument(string      id,
                         ulong       discordId,
                         string      timezoneId,
                         List<ulong> subscribedChannels) : base(id)
    {
        DiscordId          = discordId;
        TimezoneId         = timezoneId;
        SubscribedChannels = subscribedChannels;
    }

    public ulong       DiscordId          { get; init; }
    public string      TimezoneId         { get; init; }
    public List<ulong> SubscribedChannels { get; init; }

    public void Deconstruct(out string      id, out ulong discordId, out string timezoneId,
                            out List<ulong> subscribedChannels)
    {
        id                 = Id;
        discordId          = DiscordId;
        timezoneId         = TimezoneId;
        subscribedChannels = SubscribedChannels;
    }
}