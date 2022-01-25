using Discord.Commands;

namespace PierogiesBot.Discord.Infrastructure.Features.MessageSubscriptions.Handlers;

public interface IRuleMessageHandler : IObserver<CommandContext>
{
    IObservable<IResult> ResultObservable { get; }
}