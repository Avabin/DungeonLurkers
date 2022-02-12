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
using Quartz.Impl.Matchers;
using Shared.Features;
using TimeZoneConverter;

namespace PierogiesBot.Discord.Infrastructure.Features.MessageSubscriptions.Crontab;

internal class CrontabSubscribeService : ICrontabSubscribeService
{
    private readonly IBotCrontabRuleFacade            _ruleService;
    private readonly IGuildSettingFacade              _settingsService;
    private readonly IBotMessageSubscriptionFacade    _subscriptionService;
    private readonly ICrontabGuildService             _crontabGuildService;
    private readonly ILogger<CrontabSubscribeService> _logger;
    public CrontabSubscribeService(IBotCrontabRuleFacade            ruleService,
                                   IGuildSettingFacade              settingsService,
                                   IBotMessageSubscriptionFacade    subscriptionService,
                                   ICrontabGuildService             crontabGuildService,
                                   ILogger<CrontabSubscribeService> logger)
    {
        _ruleService              = ruleService;
        _settingsService          = settingsService;
        _subscriptionService      = subscriptionService;
        _crontabGuildService = crontabGuildService;
        _logger                   = logger;
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
             await _crontabGuildService.ScheduleJobForGuild(guildTimeZoneId, guildId, rule);
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
        
        _logger.LogInformation("Unsubscribed channel {Channel} in guild {Guild}", channel, guild);
    }
}