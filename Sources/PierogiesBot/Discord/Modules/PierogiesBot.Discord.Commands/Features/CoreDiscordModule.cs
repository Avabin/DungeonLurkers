using Discord.Commands;
using Microsoft.Extensions.Logging;
using PierogiesBot.Discord.Commands.Features.Logging;

namespace PierogiesBot.Discord.Commands.Features;

public class CoreDiscordModule : LoggingModuleBase<ICommandContext>
{
    public CoreDiscordModule(ILogger<CoreDiscordModule> logger) : base(logger)
    {
    }

    [Command("ping")]
    [Summary("Ping command")]
    public async Task Ping()
    {
        LogTrace("Ping");
        await ReplyAsync("Pong!");
    }
}