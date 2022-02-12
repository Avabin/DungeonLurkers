using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Discord;
using Microsoft.Extensions.Logging;
using PierogiesBot.Discord.Core.Features.MessageSubscriptions.SubscriptionServices;
using PierogiesBot.Persistence.BotCrontabRule.Features;
using PierogiesBot.Persistence.BotMessageSubscription.Features;
using PierogiesBot.Persistence.GuildSettings.Features;
using PierogiesBot.Shared.Enums;
using PierogiesBot.Shared.Features.BotCrontabRules;
using PierogiesBot.Shared.Features.BotMessageSubscriptions;
using Quartz;
using Quartz.Impl.Matchers;
using Shared.MessageBroker.Core;
using TimeZoneConverter;

namespace PierogiesBot.Discord.Infrastructure.Features.MessageSubscriptions.Crontab;

internal class CrontabSubscribeService : ICrontabSubscribeService, IDisposable
{
    private readonly IBotCrontabRuleFacade            _ruleService;
    private readonly IScheduler                       _scheduler;
    private readonly IGuildSettingFacade              _settingsService;
    private readonly IBotMessageSubscriptionFacade    _subscriptionService;
    private readonly ILogger<CrontabSubscribeService> _logger;
    private readonly IDisposable                      _sub;

    public CrontabSubscribeService(
        IScheduler                       scheduler,
        IBotCrontabRuleFacade            ruleService,
        IGuildSettingFacade              settingsService,
        IBotMessageSubscriptionFacade    subscriptionService,
        IInternalMessageBroker              messageBroker,
        ILogger<CrontabSubscribeService> logger)
    {
        _scheduler           = scheduler;
        _ruleService         = ruleService;
        _settingsService     = settingsService;
        _subscriptionService = subscriptionService;
        _logger              = logger;

        _sub = messageBroker.GetDocumentChangesObservable<BotCrontabRuleDocument>()
                            .Select(x => 
                                        Observable.Defer(() => RuleChangedAsync(x).ToObservable()))
                            .Concat()
                            .Subscribe();
    }

    private async Task RuleChangedAsync(DocumentChangeBase<BotCrontabRuleDocument, string> change)
    {
        switch (change)
        {
            case DocumentChanged<BotCrontabRuleDocument, string> x:
                await HandleRuleChangedAsync(x);
                break;
            case DocumentsChanged<BotCrontabRuleDocument, string> x:
                await HandleRulesChangedAsync(x);
                break;
        }
    }

    private async Task HandleRulesChangedAsync(DocumentsChanged<BotCrontabRuleDocument,string> rulesChanged)
    {
        switch (rulesChanged.ChangeType)
        {
            case ChangeType.Insert:
                await HandleRulesAddedAsync(rulesChanged.DocumentIds);
                break;
            case ChangeType.Delete:
                await HandleRulesRemovedAsync(rulesChanged.DocumentIds);
                break;
            case ChangeType.Update:
                await HandleRulesModifiedAsync(rulesChanged.DocumentIds);
                break;
        }
    }

    private async Task HandleRulesAddedAsync(IEnumerable<string> ruleIds)
    {
        foreach (var ruleId in ruleIds) await HandleRuleAddedAsync(ruleId);
    }

    private async Task HandleRulesRemovedAsync(IEnumerable<string> ruleIds)
    {
        foreach (var ruleId in ruleIds) await HandleRuleRemovedAsync(ruleId);
    }

    private async Task HandleRulesModifiedAsync(IEnumerable<string> ruleIds)
    {
        foreach (var ruleId in ruleIds) await HandleRuleModifiedAsync(ruleId);
    }

    private async Task HandleRuleChangedAsync(DocumentChanged<BotCrontabRuleDocument,string> ruleChanged)
    {
        switch (ruleChanged.ChangeType)
        {
            case ChangeType.Insert:
                await HandleRuleAddedAsync(ruleChanged.DocumentId);
                break;
            case ChangeType.Delete:
                await HandleRuleRemovedAsync(ruleChanged.DocumentId);
                break;
            case ChangeType.Update:
                await HandleRuleModifiedAsync(ruleChanged.DocumentId);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(ruleChanged));
        }
    }

    private async Task HandleRuleAddedAsync(string ruleId)
    {
        var rule = await _ruleService.GetByIdAsync(ruleId);
        if (rule is null) return;
        var settings = await _settingsService.GetAllAsync();
        
        foreach (var setting in settings)
        {
            var guildId = setting.GuildId;
            var guildTimeZone = setting.GuildTimeZone;

            await ScheduleJobForGuild(guildTimeZone, guildId, rule);
        }
    }

    private async Task HandleRuleRemovedAsync(string ruleId)
    {
        var ruleJobs = await _scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(ruleId));
        _logger.LogDebug("Removing {JobsCount} jobs for rule {RuleId}", ruleJobs.Count, ruleId);
        await _scheduler.DeleteJobs(ruleJobs);
    }

    private async Task HandleRuleModifiedAsync(string ruleId)
    {
        var rule = await _ruleService.GetByIdAsync(ruleId);
        if (rule is null) return;
        
        var ruleJobs = await _scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(ruleId));

        foreach (var jobKey in ruleJobs)
        {
            var jobDetail = await _scheduler.GetJobDetail(jobKey);
            
            if (jobDetail?.JobDataMap["GuildId"] is not ulong guildId) continue;
            
            var guildTimeZoneId = jobDetail.JobDataMap["GuildTimeZoneId"] as string;
            if (string.IsNullOrWhiteSpace(guildTimeZoneId)) continue;
            
            _logger.LogDebug("Deleting job {@JobKey} for guild {GuildId}", jobKey, guildTimeZoneId);
            await _scheduler.DeleteJob(jobKey);
            await ScheduleJobForGuild(guildTimeZoneId, guildId, rule);
        }
    }

    /// <inheritdoc/>
    public async Task LoadSubscriptionsAsync()
    {
        _logger.LogInformation("Loading Crontab subscriptions from DB");
        var rules = await _ruleService.GetAllAsync();
        var guilds = await _settingsService.GetAllAsync();

        var botCrontabRules = rules.ToList();
        foreach (var (guildId, guildTimeZoneId, _, _) in guilds)
        foreach (var rule in botCrontabRules)
        {
            await ScheduleJobForGuild(guildTimeZoneId, guildId, rule);
        }
    }

    private async Task ScheduleJobForGuild(string guildTimeZoneId, ulong guildId, BotCrontabRuleDto rule)
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

    /// <inheritdoc/>
    public async Task SubscribeAsync(IGuildChannel channel)
    {
        var guild = channel.Guild!;
        var existing = await _subscriptionService.GetSubscriptionForChannelAsync(channel.Id, guild.Id, SubscriptionType.Crontab);

        if (existing is null)
        {
            _logger.LogInformation(
                "Subscription not found in database. Inserting new document for channel {Channel} in guild {Guild}",
                channel, guild);
            await _subscriptionService.CreateAsync(new CreateBotMessageSubscriptionDto
            {
                ChannelId = channel.Id,
                GuildId = guild.Id,
                SubscriptionType = SubscriptionType.Crontab
            });
        }
    }

    /// <inheritdoc/>
    public async Task UnsubscribeAsync(IGuildChannel channel)
    {
        var guild = channel.Guild!;
        _logger.LogInformation("Unsubscribing channel {Channel} in guild {Guild}", channel, guild);
        var existing = await _subscriptionService.GetSubscriptionForChannelAsync(channel.Id, guild.Id, SubscriptionType.Crontab);

        var jobKeys = await _scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupContains(guild.Id.ToString()));

        await _scheduler.DeleteJobs(jobKeys);

        if (existing is not null) await _subscriptionService.DeleteAsync(existing.Id);
        
        _logger.LogInformation("Unsubscribed channel {Channel} in guild {Guild}", channel, guild);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _sub.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}