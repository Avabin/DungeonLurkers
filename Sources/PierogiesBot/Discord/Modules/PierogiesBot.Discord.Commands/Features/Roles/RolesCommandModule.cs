using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using PierogiesBot.Discord.Commands.Features.Logging;

namespace PierogiesBot.Discord.Commands.Features.Roles;

[RequireUserPermission(GuildPermission.ManageRoles)]
[Group("role")]
public class RolesCommandModule : LoggingModuleBase<ICommandContext>
{
    public RolesCommandModule(ILogger<RolesCommandModule> logger)
        : base(logger)
    {
    }

    [Command("add")]
    public async Task AddRoleToUser(IRole role, IGuildUser user)
    {
        LogTrace($"Add role {role} to user {user}");
        await user.AddRoleAsync(role);
        await ReplyAsync($"{user} now has role {role}");
    }

    [Command("remove")]
    public async Task RemoveRoleToUser(IRole role, IGuildUser user)
    {
        LogTrace($"Add role {role} to user {user}");
        await user.RemoveRoleAsync(role);
        await ReplyAsync($"{user} lost role {role}");
    }
}