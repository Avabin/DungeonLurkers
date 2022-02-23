using PierogiesBot.Shared.Enums;
using Shared.Features;

namespace PierogiesBot.Shared.Features.BotResponseRules;

public class BotResponseRuleDto : BotResponseRuleDtoBase, IDocumentDto<string>
{
    public          ResponseMode        ResponseMode            { get; set; }
    public          ICollection<string> Responses               { get; set; } = new List<string>();
    public override string              TriggerText             { get; set; } = "";
    public override StringComparison    StringComparison        { get; set; }
    public override bool                IsTriggerTextRegex      { get; set; }
    public override bool                ShouldTriggerOnContains { get; set; }
    public          string              Id                      { get; set; } = "";
}