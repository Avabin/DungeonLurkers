using Discord;
using Microsoft.Extensions.Logging;
using PierogiesBot.Persistence.BotCrontabRule.Features;
using PierogiesBot.Persistence.BotMessageSubscription.Features;
using PierogiesBot.Persistence.GuildSettings.Features;
using PierogiesBot.Shared.Enums;
using PierogiesBot.Shared.Features.BotMessageSubscriptions;
using PierogiesBot.Shared.Features.Dtos;
using Quartz;
using TimeZoneConverter;

namespace PierogiesBot.Discord.Infrastructure.Features.MessageSubscriptions.Crontab;

internal class CrontabSubscribeService : ICrontabSubscribeService
{
    private readonly IBotCrontabRuleFacade _ruleService;
    private readonly IScheduler _scheduler;
    private readonly IGuildSettingFacade _settingsService;
    private readonly IBotMessageSubscriptionFacade _subscriptionService;
    private readonly ILogger<CrontabSubscribeService> _logger;

    public CrontabSubscribeService(
        IScheduler                       scheduler,
        IBotCrontabRuleFacade            ruleService,
        IGuildSettingFacade              settingsService,
        IBotMessageSubscriptionFacade    subscriptionService,
        ILogger<CrontabSubscribeService> logger)
    {
        _scheduler = scheduler;
        _ruleService = ruleService;
        _settingsService = settingsService;
        _subscriptionService = subscriptionService;
        _logger = logger;
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

        if (existing is not null) await _subscriptionService.DeleteAsync(existing.Id);
    }
}