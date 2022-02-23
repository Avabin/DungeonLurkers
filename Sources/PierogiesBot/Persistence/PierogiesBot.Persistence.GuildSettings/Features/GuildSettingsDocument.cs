using MongoDB.Bson;
using Shared.Persistence.Core.Features.Documents;

namespace PierogiesBot.Persistence.GuildSettings.Features;

public record GuildSettingDocument(string Id, ulong GuildId, string GuildTimeZone, ulong GuildMuteRoleId) : DocumentBase<string>(Id)
{
    public GuildSettingDocument(ulong guildId, string guildTimeZone, ulong guildMuteRoleId)
        : this(ObjectId.GenerateNewId().ToString(), guildId, guildTimeZone, guildMuteRoleId)
    {
    }
}