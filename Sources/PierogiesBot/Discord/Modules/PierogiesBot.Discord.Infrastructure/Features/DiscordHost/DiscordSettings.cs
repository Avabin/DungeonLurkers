namespace PierogiesBot.Discord.Infrastructure.Features.DiscordHost;

public class DiscordSettings
{
    public const string SectionName = nameof(DiscordSettings);

    public string Token        { get; set; } = "";
    public string DicesBaseUrl { get; set; } = "";
}