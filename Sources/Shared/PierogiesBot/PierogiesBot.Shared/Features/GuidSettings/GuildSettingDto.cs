using Shared.Features;

namespace PierogiesBot.Shared.Features.GuidSettings
{
    
    public class GuildSettingDto : GuildSettingsDtoBase, IDocumentDto<string>
    {
        public void Deconstruct(out ulong guildId, out string guildTimeZone, out ulong guildMuteRoleId, out string id)
        {
            guildId = GuildId;
            guildTimeZone = GuildTimeZone;
            guildMuteRoleId = GuildMuteRoleId;
            id = Id;
        }
        public string Id { get; set; } = "";
    }
}