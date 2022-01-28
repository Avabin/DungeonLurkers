using Discord.Commands;

namespace PierogiesBot.Discord.Core.Features.MessageSubscriptions.Handlers;

public interface IUserSocketMessageHandler
{
    Task<IResult> HandleAsync(SocketCommandContext messageContext);
}