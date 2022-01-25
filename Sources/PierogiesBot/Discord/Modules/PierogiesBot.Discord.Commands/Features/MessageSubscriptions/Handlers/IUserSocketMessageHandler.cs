using Discord.Commands;

namespace PierogiesBot.Discord.Commands.Features.MessageSubscriptions.Handlers;

public interface IUserSocketMessageHandler
{
    Task<IResult> HandleAsync(SocketCommandContext messageContext);
}