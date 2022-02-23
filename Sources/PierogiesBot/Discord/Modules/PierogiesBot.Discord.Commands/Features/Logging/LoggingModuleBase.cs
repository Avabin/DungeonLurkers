using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;

namespace PierogiesBot.Discord.Commands.Features.Logging;

public abstract class LoggingModuleBase<TCommandContext> : ModuleBase<TCommandContext> where TCommandContext : class, ICommandContext
{
    private readonly ILogger<LoggingModuleBase<TCommandContext>> _logger;

    protected LoggingModuleBase(ILogger<LoggingModuleBase<TCommandContext>> logger)
    {
        _logger = logger;
    }

    protected void LogTrace(string message, IUser user, IGuild guild, IChannel channel)
    {
        _logger.LogTrace("<{Guild}|{Channel}|{User}> {Message}", guild, channel, user, message);
    }

    protected void LogTrace(string message)
    {
        LogTrace(message, Context.User, Context.Guild, Context.Channel);
    }
        
    protected void LogError(Exception? e, string message = "")
    {
        LogError(e, Context.User, Context.Guild, Context.Channel, message);
    }
        
    protected void LogError(Exception? e, IUser user, IGuild guild, IChannel channel, string message = "")
    {
        _logger.LogError(e, "<{Guild}|{Channel}|{User}> {Message}", guild, channel, user, message);
    }
}