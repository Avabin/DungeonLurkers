using PierogiesBot.Shared.Enums;

namespace PierogiesBot.Shared.Features.Dtos;


public class DiscordGuildIdAndSubscriptionType
{
    
    public ulong GuildId { get; set; }
    
    public SubscriptionType SubscriptionType { get; set; }
}