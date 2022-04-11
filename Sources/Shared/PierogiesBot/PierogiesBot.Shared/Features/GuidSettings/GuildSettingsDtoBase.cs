namespace PierogiesBot.Shared.Features.GuidSettings;

public abstract class GuildSettingsDtoBase
{
    public virtual void Deconstruct(out ulong guildId, out string guildTimeZone, out ulong guildMuteRoleId)
    {
        guildId         = GuildId;
        guildTimeZone   = GuildTimeZone;
        guildMuteRoleId = GuildMuteRoleId;
    }

    public ulong  GuildId         { get; set; }
    public string GuildTimeZone   { get; set; } = "";
    public ulong  GuildMuteRoleId { get; set; }
}