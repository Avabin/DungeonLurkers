using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using PierogiesBot.Discord.Commands.Features.Logging;
using PierogiesBot.Discord.Core.Features.MessageSubscriptions.SubscriptionServices;

namespace PierogiesBot.Discord.Commands.Features.MessageSubscriptions;

[RequireUserPermission(GuildPermission.Administrator)]
[Group("sub")]
public class SubscribeCommandModule : LoggingModuleBase<ICommandContext>
{
    public SubscribeCommandModule(ILogger<SubscribeCommandModule> logger)
        : base(logger)
    {
    }

    [Group("responses")]
    public class SubscribeResponsesCommandModule : LoggingModuleBase<ICommandContext>
    {
        private readonly IChannelSubscribeService                 _channelSubscribeService;
        private readonly ILogger<SubscribeResponsesCommandModule> _logger;

        public SubscribeResponsesCommandModule(ILogger<SubscribeResponsesCommandModule> logger,
                                               IChannelSubscribeService channelSubscribeService) : base(logger)
        {
            _logger                  = logger;
            _channelSubscribeService = channelSubscribeService;
        }

        [Command("all")]
        public async Task Subscribe()
        {
            LogTrace("Subscribe to all channels");
            _logger.LogInformation("New response subscription on channel {Channel} guild {Guild}", Context.Channel,
                                   Context.Guild);

            foreach (var guildChannel in await Context.Guild.GetTextChannelsAsync())
                await _channelSubscribeService.SubscribeAsync(guildChannel);

            await ReplyAsync("I will watch ALL channels from now on...");
        }

        [Command("add")]
        public async Task Subscribe(IGuildChannel channel)
        {
            LogTrace($"Subscribe to channel {channel}");
            _logger.LogInformation("New response subscription on channel {Channel} guild {Guild}", Context.Channel,
                                   Context.Guild);
            await _channelSubscribeService.SubscribeAsync(channel);

            await ReplyAsync($"I will watch channel {channel.Name} from now on...");
        }

        [Command("add")]
        public async Task Subscribe(params IGuildChannel[] channels)
        {
            LogTrace($"Subscribe to channels {string.Join(", ", channels.Select(x => x.Name))}");
            _logger.LogInformation("New response subscription on  multiple channels in guild {Guild}",
                                   Context.Guild);

            foreach (var channel in channels)
                await _channelSubscribeService.SubscribeAsync(channel);

            await ReplyAsync(
                             $"I will watch channels {string.Join(", ", channels.Select(c => c.Name))} from now on...");
        }

        [Command("del")]
        public async Task Unsubscribe()
        {
            LogTrace("Unsubscribing all channels");
            _logger.LogInformation("Del response subscription on guild {Guild}", Context.Guild);

            foreach (var guildChannel in await Context.Guild.GetTextChannelsAsync())
                await _channelSubscribeService.UnsubscribeAsync(guildChannel);

            await ReplyAsync("I got bored watching you, bye");
        }

        [Command("del")]
        public async Task Unsubscribe(params IGuildChannel[] channels)
        {
            LogTrace($"Unsubscribing channels {string.Join(", ", channels.Select(x => x.Name))}");

            foreach (var channel in channels) await _channelSubscribeService.UnsubscribeAsync(channel);

            await ReplyAsync($"I got bored watching you on {string.Join(", ", channels.Select(x => x.Name))}, bye");
        }
    }

    [Group("crontab")]
    public class SubscribeCrontabCommandModule : LoggingModuleBase<ICommandContext>
    {
        private readonly ICrontabSubscribeService               _crontabSubscribeService;
        private readonly ILogger<SubscribeCrontabCommandModule> _logger;

        public SubscribeCrontabCommandModule(ILogger<SubscribeCrontabCommandModule> logger,
                                             ICrontabSubscribeService crontabSubscribeService) : base(logger)
        {
            _logger                  = logger;
            _crontabSubscribeService = crontabSubscribeService;
        }

        [Command("all")]
        public async Task Subscribe()
        {
            LogTrace($"New Crontab subscription to all channels");
            _logger.LogInformation("New crontab subscription on channel {Channel} guild {Guild}", Context.Channel,
                                   Context.Guild);

            foreach (var guildChannel in await Context.Guild.GetChannelsAsync())
                await _crontabSubscribeService.SubscribeAsync(guildChannel);

            await ReplyAsync(
                             $"I will post scheduled messages on all channels from now on");
        }

        [Command("add")]
        public async Task Subscribe(IGuildChannel channel)
        {
            LogTrace($"New Crontab subscription to channel {channel}");
            _logger.LogInformation("New crontab subscription on channel {Channel} guild {Guild}", Context.Channel,
                                   Context.Guild);
            await _crontabSubscribeService.SubscribeAsync(channel);

            await ReplyAsync(
                             $"I will post scheduled messages on {channel} from now on");
        }

        [Command("add")]
        public async Task Subscribe(params IGuildChannel[] channels)
        {
            LogTrace($"New Crontab subscription to channels {string.Join(",", channels.Select(x => x.Name))}");
            _logger.LogInformation("New crontab subscription on multiple channels in guild {Guild}",
                                   Context.Guild);

            if (!channels.Any())
            {
                var channel = await Context.Guild.GetChannelAsync(Context.Channel.Id);
                await _crontabSubscribeService.SubscribeAsync(channel);

                await ReplyAsync($"I will post scheduled messages in {channel}");
                return;
            }

            foreach (var channel in channels)
                await _crontabSubscribeService.SubscribeAsync(channel);

            await ReplyAsync(
                             $"I will post scheduled messages on {string.Join(", ", channels.Select(c => c.ToString()))} from now on");
        }

        [Command("del")]
        public async Task Unsubscribe()
        {
            LogTrace("Unsubscribing from all channels");
            _logger.LogInformation("Del crontab subscription on guild {Guild}", Context.Guild);

            foreach (var guildChannel in await Context.Guild.GetChannelsAsync())
                if (guildChannel is IGuildChannel channel)
                    await _crontabSubscribeService.UnsubscribeAsync(channel);

            await ReplyAsync("I got bored posting here, bye");
        }

        [Command("del")]
        public async Task Unsubscribe(IGuildChannel channel)
        {
            LogTrace($"Unsubscribing from {channel}");
            _logger.LogInformation("Del response subscription on channel {Channel} guild {Guild}", Context.Channel,
                                   Context.Guild);

            await _crontabSubscribeService.UnsubscribeAsync(channel);

            await ReplyAsync($"I got bored posting on {channel}, bye");
        }

        [Command("del")]
        public async Task Unsubscribe(params IGuildChannel[] channels)
        {
            LogTrace($"Unsubscribing channels {string.Join(", ", channels.Select(x => x.Name))}");

            foreach (var channel in channels) await _crontabSubscribeService.UnsubscribeAsync(channel);

            await ReplyAsync($"I got bored posting on {string.Join(", ", channels.Select(x => x.Name))}, bye");
        }
    }
}