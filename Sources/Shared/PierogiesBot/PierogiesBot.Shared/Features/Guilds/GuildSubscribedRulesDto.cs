namespace PierogiesBot.Shared.Features.Guilds;

public class GuildSubscribedRulesDto
{
    public string      GuildId         { get; set; } = "";
    public List<string> SubscribedRules { get; set; } = new();
    public RuleType    RuleType        { get; set; } = RuleType.Unknown;
}