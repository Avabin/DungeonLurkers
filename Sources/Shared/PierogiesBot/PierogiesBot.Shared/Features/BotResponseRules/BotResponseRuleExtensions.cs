using System.Text.RegularExpressions;
using PierogiesBot.Shared.Features.Dtos;

namespace PierogiesBot.Shared.Features.BotResponseRules;

public static class BotResponseRuleExtensions
{
    public static bool CanExecuteRule(this IBotSubscriptionRule rule, string message)
    {
        var isRegex = rule.IsTriggerTextRegex;
        var triggerOnContains = rule.ShouldTriggerOnContains;

        return (isRegex, triggerOnContains) switch
        {
            (true, true) => ContainsMatchRegex(message, rule),
            (true, false) => IsMatchRegex(message, rule),
            (false, false) => IsMatchText(message, rule),
            (false, true) => ContainsText(message, rule)
        };
    }

    private static bool ContainsMatchRegex(string message, IBotSubscriptionRule rule) => 
        Regex.IsMatch(message, rule.TriggerText);

    private static bool IsMatchRegex(string message, IBotSubscriptionRule rule) => 
        Regex.IsMatch(message, $"^{rule.TriggerText}$");

    private static bool IsMatchText(string message, IBotSubscriptionRule rule) => 
        message.Equals(rule.TriggerText, rule.StringComparison);

    private static bool ContainsText(string message, IBotSubscriptionRule rule) => 
        message.Contains(rule.TriggerText, rule.StringComparison);
}