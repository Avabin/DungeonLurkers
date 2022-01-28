using Discord.Commands;

namespace PierogiesBot.Discord.Core.Features.MessageSubscriptions.Handlers;

public interface IRuleMessageHandler : IObserver<SocketCommandContext>
{
    IObservable<IResult> ResultObservable { get; }
}