using PierogiesBot.Shared.Enums;
using Shared.Features;

namespace PierogiesBot.Shared.Features.BotCrontabRules
{
    
    public class UpdateBotCrontabRuleDto : BotCrontabRuleDtoBase, IUpdateDocumentDto
    {
        public bool                IsEmoji       { get; set; }
        public string              Crontab       { get; set; } = "";
        public ICollection<string> ReplyMessages { get; set; } = new List<string>();
        public ICollection<string> ReplyEmojis   { get; set; } = new List<string>();
        public ResponseMode        ResponseMode  { get; set; }
    }
}