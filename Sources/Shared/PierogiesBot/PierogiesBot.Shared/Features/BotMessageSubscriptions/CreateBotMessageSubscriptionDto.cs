using PierogiesBot.Shared.Enums;
using Shared.Features;

namespace PierogiesBot.Shared.Features.BotMessageSubscriptions
{
    
    public class CreateBotMessageSubscriptionDto : BotMessageSubscriptionDtoBase, ICreateDocumentDto
    {
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public SubscriptionType SubscriptionType { get; set; }
    }
}