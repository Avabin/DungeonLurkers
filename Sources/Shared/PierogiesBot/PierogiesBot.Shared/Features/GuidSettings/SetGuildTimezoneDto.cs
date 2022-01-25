using PierogiesBot.Shared.Features.Dtos;
using Shared.Features;

namespace PierogiesBot.Shared.Features.GuidSettings;


public class SetGuildTimezoneDto : IUpdateDocumentDto
{

    public string Timezone { get; set; } = "";
    public ulong GuildId { get; set; }
}