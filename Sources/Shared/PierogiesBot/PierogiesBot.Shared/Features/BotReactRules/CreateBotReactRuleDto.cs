using PierogiesBot.Shared.Enums;
using PierogiesBot.Shared.Features.Dtos;
using Shared.Features;

namespace PierogiesBot.Shared.Features.BotReactRules
{
    
    public class CreateBotReactRuleDto : BotReactRuleDtoBase, ICreateDocumentDto
    {
        public          ICollection<string> Reactions               { get; set; } = new List<string>();
        public override string              TriggerText             { get; set; } = "";
        public override StringComparison    StringComparison        { get; set; }
        public override bool                IsTriggerTextRegex      { get; set; }
        public override bool                ShouldTriggerOnContains { get; set; }
        public          ResponseMode        ResponseMode            { get; set; }
    }
}