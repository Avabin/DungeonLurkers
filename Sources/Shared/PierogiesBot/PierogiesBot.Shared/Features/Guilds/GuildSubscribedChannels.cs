namespace PierogiesBot.Shared.Features.Guilds;

public class GuildSubscribedChannelsDto
{
    public string      GuildId            { get; set; } = "";
    public List<ulong> SubscribedChannels { get; set; } = new();
}