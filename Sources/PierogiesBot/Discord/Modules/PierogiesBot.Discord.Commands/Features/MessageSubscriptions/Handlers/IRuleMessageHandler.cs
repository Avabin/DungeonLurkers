using Discord.Commands;

namespace PierogiesBot.Discord.Commands.Features.MessageSubscriptions.Handlers;

public interface IRuleMessageHandler : IObserver<SocketCommandContext>
{
    IObservable<IResult> ResultObservable { get; }
}