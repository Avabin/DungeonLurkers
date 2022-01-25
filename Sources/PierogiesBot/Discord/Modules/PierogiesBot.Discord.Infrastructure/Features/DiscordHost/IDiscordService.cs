using Discord.WebSocket;

namespace PierogiesBot.Discord.Infrastructure.Features.DiscordHost;

public interface IDiscordService
{
    IObservable<SocketMessage> MessageObservable { get; }
    Task Start();
    Task Stop();
}