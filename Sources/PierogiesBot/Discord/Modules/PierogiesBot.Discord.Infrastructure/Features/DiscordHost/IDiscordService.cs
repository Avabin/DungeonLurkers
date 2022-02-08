using Discord.Commands;
using Discord.WebSocket;

namespace PierogiesBot.Discord.Infrastructure.Features.DiscordHost;

public interface IDiscordService
{
    IObservable<SocketMessage> MessageObservable { get; }

    SocketGuild?        GetGuild(ulong   guildId);
    SocketGuildChannel? GetChannel(ulong channelId, ulong guildId);
    Task                Start();
    Task                Stop();
}