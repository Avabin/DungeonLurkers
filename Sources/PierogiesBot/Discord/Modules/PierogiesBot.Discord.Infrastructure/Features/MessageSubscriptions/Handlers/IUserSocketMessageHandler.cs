using Discord.Commands;

namespace PierogiesBot.Discord.Infrastructure.Features.MessageSubscriptions.Handlers;

public interface IUserSocketMessageHandler
{
    Task<IResult> HandleAsync(CommandContext messageContext);
}