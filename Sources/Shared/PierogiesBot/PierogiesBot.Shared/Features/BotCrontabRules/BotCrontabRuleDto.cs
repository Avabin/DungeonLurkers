using PierogiesBot.Shared.Enums;
using PierogiesBot.Shared.Features.Dtos;
using Shared.Features;

namespace PierogiesBot.Shared.Features.BotCrontabRules
{
    
    public class BotCrontabRuleDto : BotCrontabRuleDtoBase, IDocumentDto<string>
    {
        public bool                IsEmoji       { get; set; }
        public string              Crontab       { get; set; } = "";
        public ICollection<string> ReplyMessages { get; set; } = new List<string>();
        public ICollection<string> ReplyEmojis   { get; set; } = new List<string>();
        public ResponseMode        ResponseMode  { get; set; }
        public string              Id            { get; set; } = "";
    }
}