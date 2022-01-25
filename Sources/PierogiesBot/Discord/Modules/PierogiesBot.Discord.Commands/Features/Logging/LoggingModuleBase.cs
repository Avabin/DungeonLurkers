using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace PierogiesBot.Discord.Commands.Features.Logging
{
    public abstract class LoggingModuleBase : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger<LoggingModuleBase> _logger;

        protected LoggingModuleBase(ILogger<LoggingModuleBase> logger)
        {
            _logger = logger;
        }

        protected void LogTrace(string message, SocketUser user, SocketGuild guild, ISocketMessageChannel channel)
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
        
        protected void LogError(Exception? e, SocketUser user, SocketGuild guild, ISocketMessageChannel channel, string message = "")
        {
            _logger.LogError(e, "<{Guild}|{Channel}|{User}> {Message}", guild, channel, user, message);
        }
    }
}