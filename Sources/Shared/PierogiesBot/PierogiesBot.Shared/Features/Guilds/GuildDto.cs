using Shared.Features;

namespace PierogiesBot.Shared.Features.Guilds;

public class GuildDto : GuildDtoBase, IDocumentDto<string>
{
    public string Id { get; } = "";
}