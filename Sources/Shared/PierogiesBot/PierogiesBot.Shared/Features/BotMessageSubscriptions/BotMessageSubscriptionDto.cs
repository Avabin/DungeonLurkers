using PierogiesBot.Shared.Enums;
using Shared.Features;

namespace PierogiesBot.Shared.Features.BotMessageSubscriptions
{
    
    public class BotMessageSubscriptionDto : BotMessageSubscriptionDtoBase, IDocumentDto<string>
    {
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public SubscriptionType SubscriptionType { get; set; }
        public string Id { get; set; } = "";
    }
}