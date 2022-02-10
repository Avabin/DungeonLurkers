using Shared.Features;

namespace PierogiesBot.Shared.Features.GuidSettings;


public class SetGuildMuteRole : IUpdateDocumentDto
{
    public ulong GuildMuteRoleId { get; set; }
    public ulong GuildId { get; set; }
}