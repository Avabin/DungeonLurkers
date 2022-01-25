using Discord.Commands;
using Microsoft.Extensions.Logging;
using PierogiesBot.Discord.Infrastructure.Features.Logging;

namespace PierogiesBot.Discord.Infrastructure.Features;

public class CoreDiscordModule : LoggingModuleBase
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