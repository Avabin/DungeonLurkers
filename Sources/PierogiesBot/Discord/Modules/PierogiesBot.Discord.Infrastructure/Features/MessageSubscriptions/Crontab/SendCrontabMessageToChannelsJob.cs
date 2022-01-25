using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using PierogiesBot.Persistence.BotMessageSubscription.Features;
using PierogiesBot.Shared.Enums;
using PierogiesBot.Shared.Features.BotCrontabRules;
using PierogiesBot.Shared.Features.BotMessageSubscriptions;
using PierogiesBot.Shared.Features.Dtos;
using Quartz;

namespace PierogiesBot.Discord.Infrastructure.Features.MessageSubscriptions.Crontab;

public class SendCrontabMessageToChannelsJob : IJob
{
    private readonly DiscordSocketClient _client;
    private readonly ILogger<SendCrontabMessageToChannelsJob> _logger;
    private readonly IBotMessageSubscriptionFacade _subscriptions;

    public SendCrontabMessageToChannelsJob(ILogger<SendCrontabMessageToChannelsJob> logger,
        DiscordSocketClient client, IBotMessageSubscriptionFacade subscriptions)
    {
        _logger = logger;
        _client = client;
        _subscriptions = subscriptions;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        if (context.MergedJobDataMap["Rule"] is BotCrontabRuleDto rule &&
            context.MergedJobDataMap["GuildId"] is ulong guildId)
        {
            _logger.LogDebug("Running job trigger fron crontab rule {Crontab}", rule.Crontab);
            var subs = await _subscriptions.GetByGuildAndSubscriptionTypeAsync(guildId, SubscriptionType.Crontab);

            var subsList = subs.ToList();
            if (subsList.Any())
            {
                foreach (var sub in subsList)
                {
                    await HandleSubscription(sub, rule);
                }
            }
        }
    }

    private async Task HandleSubscription(BotMessageSubscriptionDto sub, BotCrontabRuleDto rule)
    {
        var channel = (ITextChannel) _client.GetChannel(sub.ChannelId);
        var guild = channel.Guild;
        if (rule.IsEmoji)
        {
            var foundEmotes = guild.Emotes.FirstOrDefault(x => x.Name.Equals(rule.ReplyEmojis.First()));

            if (foundEmotes is not null)
                await channel.SendMessageAsync(foundEmotes.ToString());
        }
        else
        {
            await channel.SendMessageAsync(rule.ReplyMessages.First());
        }
    }
}