namespace PierogiesBot.Shared.Features.Guilds;

public abstract class GuildDtoBase
{
    public ulong  DiscordId  { get; set; }
    public string TimezoneId { get; set; } = "";
    public List<ulong> SubscribedChannels { get; set; } = new();
}