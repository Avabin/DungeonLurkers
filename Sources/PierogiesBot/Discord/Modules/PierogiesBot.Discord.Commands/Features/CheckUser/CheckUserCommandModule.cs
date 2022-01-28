using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using PierogiesBot.Discord.Commands.Features.Logging;

namespace PierogiesBot.Discord.Commands.Features.CheckUser
{
    public class CheckUserCommandModule : LoggingModuleBase<ICommandContext>
    {
        public CheckUserCommandModule(ILogger<CheckUserCommandModule> logger) : base(logger)
        {
        }

        [Command("whois")]
        public Task WhoIs(IUser? user = null)
        {
            LogTrace($"Whois {user}");
            user ??= Context.Client.CurrentUser;

            return ReplyAsync($"{user.Username}#{user.Discriminator}");
        }
    }
}