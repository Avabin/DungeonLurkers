namespace PierogiesBot.Shared.Features.Guilds;

public abstract class GuildDtoBase
{
    public ulong        DiscordId               { get; set; }
    public string       Name                    { get; set; } = "";
    public string       IconUri                 { get; set; } = "";
    public string       TimezoneId              { get; set; } = "";
    public List<ulong>  SubscribedChannels      { get; set; } = new();
    public List<string> SubscribedCrontabRules  { get; set; } = new();
    public List<string> SubscribedResponseRules { get; set; } = new();
    public List<string> SubscribedReactionRules { get; set; } = new();
}