using AutoMapper;
using Microsoft.Extensions.Logging;
using PierogiesBot.Persistence.BotCrontabRule.Features;
using PierogiesBot.Persistence.GuildSettings.Features;
using PierogiesBot.Shared.Features.BotCrontabRules;
using Quartz;
using Quartz.Impl.Matchers;
using TimeZoneConverter;

namespace PierogiesBot.Discord.Infrastructure.Features.MessageSubscriptions.Crontab;

public class CrontabGuildService : ICrontabGuildService
{
    private readonly IScheduler                   _scheduler;
    private readonly IMapper                      _mapper;
    private readonly IGuildSettingFacade          _settingFacade;
    private readonly ILogger<CrontabGuildService> _logger;

    public CrontabGuildService(IScheduler scheduler, IMapper mapper, IGuildSettingFacade settingFacade, ILogger<CrontabGuildService> logger)
    {
        _scheduler     = scheduler;
        _mapper   = mapper;
        _settingFacade = settingFacade;
        _logger        = logger;
    }
    public async Task RemoveJobsForRule(string ruleId)
    {
        var ruleJobs = await _scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(ruleId));

        await _scheduler.PauseTriggers(GroupMatcher<TriggerKey>.GroupContains(ruleId));
        await _scheduler.DeleteJobs(ruleJobs);
    }

    public async Task RemoveJobsForGuild(ulong guildId)
    {
        await _scheduler.PauseTriggers(GroupMatcher<TriggerKey>.GroupContains(guildId.ToString()));
        var jobKeys = await _scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupContains(guildId.ToString()));
        
        await _scheduler.DeleteJobs(jobKeys);
    }

    public async Task RescheduleJobsForRule(BotCrontabRuleDocument rule)
    {
        var jobKeys = await _scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupContains(rule.Id));

        foreach (var jobKey in jobKeys)
        {
            var job             = await _scheduler.GetJobDetail(jobKey);
            var maybeGuildId    = job?.JobDataMap["GuildId"] as ulong?;
            var guildTimeZoneId = job?.JobDataMap["GuildTimeZoneId"] as string;

            if (job?.JobDataMap["Rule"] is not BotCrontabRuleDto) return;
            if (maybeGuildId is not { } guildId) return;
            if (string.IsNullOrWhiteSpace(guildTimeZoneId)) return;
            
            var mapped = _mapper.Map<BotCrontabRuleDto>(rule);
            await _scheduler.DeleteJob(jobKey);
            await ScheduleJobForGuild(guildTimeZoneId, guildId, mapped);
        }
    }

    public async Task ScheduleJobsForRule(BotCrontabRuleDocument rule)
    {
        var guildSettings = await _settingFacade.GetAllAsync();
        var ruleDto = _mapper.Map<BotCrontabRuleDto>(rule);
        
        foreach (var (guildId, guildTimeZoneId, _) in guildSettings)
            await ScheduleJobForGuild(guildTimeZoneId, guildId, ruleDto);
    }

    public async Task ScheduleJobForGuild(string guildTimeZoneId, ulong guildId, BotCrontabRuleDto rule)
    {
        var tzInfo = TZConvert.GetTimeZoneInfo(guildTimeZoneId);

        _logger.LogInformation("Creating job for guild {{{GuildId}}} in TimeZone '{TimeZone}', Crontab = {{{Crontab}}}",
                               guildId,
                               tzInfo.DisplayName,
                               rule.Crontab);

        var guildIdS = guildId.ToString();
        var job = JobBuilder.Create<SendCrontabMessageToChannelsJob>()
                            .WithIdentity(guildIdS, rule.Id)
                            .SetJobData(new JobDataMap
                             {
                                 { "Rule", rule },
                                 { "GuildId", guildId },
                                 { "GuildTimeZoneId", guildTimeZoneId }
                             }).Build();

        var trigger = TriggerBuilder
                     .Create()
                     .WithIdentity(guildIdS, rule!.Id)
                     .ForJob(job)
                     .WithCronSchedule(rule.Crontab, builder => builder.InTimeZone(tzInfo))
                     .Build();

        await _scheduler.ScheduleJob(job, trigger);

        var triggerNextFire = trigger.GetNextFireTimeUtc();
        _logger.LogDebug("Trigger '{Crontab}' next fire time is {NextFire:F}", rule.Crontab, triggerNextFire);
    }
}