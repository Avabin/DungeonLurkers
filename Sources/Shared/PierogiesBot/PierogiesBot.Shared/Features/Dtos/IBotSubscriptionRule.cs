namespace PierogiesBot.Shared.Features.Dtos;

public interface IBotSubscriptionRule
{
    bool IsTriggerTextRegex { get; set; }
    bool ShouldTriggerOnContains { get; set; }
    string TriggerText { get; set; }
    StringComparison StringComparison { get; set; }
}