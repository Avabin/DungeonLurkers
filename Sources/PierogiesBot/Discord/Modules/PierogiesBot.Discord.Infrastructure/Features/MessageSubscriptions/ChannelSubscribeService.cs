﻿using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using PierogiesBot.Discord.Core.Features.MessageSubscriptions.Handlers;
using PierogiesBot.Discord.Core.Features.MessageSubscriptions.SubscriptionServices;
using PierogiesBot.Persistence.BotMessageSubscription.Features;
using PierogiesBot.Shared.Enums;
using PierogiesBot.Shared.Features.BotMessageSubscriptions;

namespace PierogiesBot.Discord.Infrastructure.Features.MessageSubscriptions;

public class ChannelSubscribeService : IChannelSubscribeService
{
    private readonly DiscordSocketClient                     _client;
    private readonly IRuleMessageHandler                     _handler;
    private readonly IBotMessageSubscriptionFacade           _dataService;
    private readonly Dictionary<(ulong, ulong), IDisposable> _subscriptions;
    private readonly ILogger<ChannelSubscribeService>        _logger;
    private readonly IObservable<SocketMessage>              _messageObservable;
    private readonly ISubject<SocketMessage>                 _messageSubject;

    public ChannelSubscribeService(
        IRuleMessageHandler              handler,
        IBotMessageSubscriptionFacade    dataService,
        DiscordSocketClient              client,
        ILogger<ChannelSubscribeService> logger)
    {
        _handler     = handler;
        _dataService = dataService;
        _client      = client;
        _logger      = logger;

        _subscriptions     = new Dictionary<(ulong, ulong), IDisposable>();
        _messageSubject    = new Subject<SocketMessage>();
        _messageObservable = _messageSubject.AsObservable();

        client.MessageReceived += message => Task.Run(() => _messageSubject.OnNext(message));
    }

    /// <inheritdoc/>
    public async Task LoadSubscriptionsAsync()
    {
        
        _logger.LogInformation("Loading subscriptions from DB");
        var subscriptions = await _dataService.GetAllAsync();

        foreach (var sub in subscriptions)
        {
            var guildId   = sub.GuildId;
            var channelId = sub.ChannelId;

            var guild = _client.GetGuild(guildId);

            if (guild is null)
            {
                _logger.LogWarning("Guild with Id {0} not found!", guildId);
                continue;
            }

            _logger.LogDebug("Found guild [{0}]", guild.Name);
            var channel = guild.GetTextChannel(channelId);

            if (channel is null)
            {
                _logger.LogWarning("Text channel with Id {0} not found!", guildId);
                continue;
            }

            _logger.LogDebug("Found channel [{0}] in guild [{1}]", channel.Name, guild.Name);
            await SubscribeAsync(channel, sub);
        }
    }

    /// <inheritdoc/>
    public Task SubscribeAsync(IGuildChannel channel) => SubscribeAsync(channel, null);

    public async Task SubscribeAsync(IGuildChannel channel, BotMessageSubscriptionDto? subscriptionDto)
    {
        var guild    = channel.Guild;
        var guildS   = guild.ToString();
        var channelS = channel.ToString();
        _logger.LogTrace("Subscribing to channel {0} in guild {1}", channelS, guildS);
        var existing = subscriptionDto ??
                       await _dataService.GetSubscriptionForChannelAsync(channel.Id, guild.Id,
                                                                         SubscriptionType.Responses);

        if (existing is null)
        {
            _logger.LogTrace("Not found any existing subscription. Creating new in database");
            await _dataService.CreateAsync(new CreateBotMessageSubscriptionDto
            {
                ChannelId        = channel.Id,
                GuildId          = guild.Id,
                SubscriptionType = SubscriptionType.Responses
            });
        }

        var guildId   = guild.Id;
        var channelId = channel.Id;

        if (_subscriptions.ContainsKey((guildId, channelId)))
        {
            _logger.LogTrace("Already subscribed to channel with id {GuildId} in guild with id {ChannelId}",
                             guildS,
                             channelS);
            return;
        }

        _logger.LogTrace("Creating new observable subscription to channel with id {GuildId} in guild with id {ChannelId}",
                         guildS,
                         channelS);

        var disposable = _messageObservable
                        .ObserveOn(TaskPoolScheduler.Default)
                        .Select(m => m as SocketUserMessage)
                        .Where(m => m is not null)
                        .Where(m => m is { Channel: { } c, Author.IsBot: false } && c.Id == channelId)
                        .Select(m => new SocketCommandContext(_client, m))
                        .Subscribe(_handler);

        _subscriptions[(guildId, channelId)] = disposable;
    }

    /// <inheritdoc/>
    public async Task UnsubscribeAsync(IGuildChannel channel)
    {
        var guild    = channel.Guild;
        var guildS   = guild.ToString();
        var channelS = channel.ToString();
        _logger.LogTrace("Unsubscribing from channel {ChannelId} in guild {GuildId}", channelS, guildS);

        var existing =
            await _dataService.GetSubscriptionForChannelAsync(channel.Id, guild.Id, SubscriptionType.Responses);

        if (existing is not null)
        {
            await _dataService.DeleteAsync(existing.Id);
            if (_subscriptions[(guild.Id, channel.Id)] is { } sub) sub.Dispose();
        }
    }
}