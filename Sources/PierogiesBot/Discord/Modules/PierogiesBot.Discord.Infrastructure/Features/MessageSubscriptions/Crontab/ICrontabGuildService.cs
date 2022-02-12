using PierogiesBot.Persistence.BotCrontabRule.Features;
using PierogiesBot.Shared.Features.BotCrontabRules;

namespace PierogiesBot.Discord.Infrastructure.Features.MessageSubscriptions.Crontab;

public interface ICrontabGuildService
{
    Task RemoveJobsForRule(string                     ruleId);
    Task RemoveJobsForGuild(ulong                     guildId);
    Task RescheduleJobsForRule(BotCrontabRuleDocument rule);
    Task ScheduleJobsForRule(BotCrontabRuleDocument   rule);
    Task ScheduleJobForGuild(string                   guildTimeZoneId, ulong guildId, BotCrontabRuleDto rule);
}