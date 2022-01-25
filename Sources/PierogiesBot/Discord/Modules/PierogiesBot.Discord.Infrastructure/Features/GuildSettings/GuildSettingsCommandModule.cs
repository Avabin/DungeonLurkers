using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using PierogiesBot.Discord.Infrastructure.Features.Logging;
using PierogiesBot.Persistence.GuildSettings.Features;
using PierogiesBot.Shared.Features.Dtos;
using PierogiesBot.Shared.Features.GuidSettings;

namespace PierogiesBot.Discord.Infrastructure.Features.GuildSettings;

[RequireUserPermission(GuildPermission.Administrator)]
[Group("settings")]
public class GuildSettingsCommandModule : LoggingModuleBase
{
    private readonly IGuildSettingFacade _settingsService;

    public GuildSettingsCommandModule(
        ILogger<GuildSettingsCommandModule> logger,
        IGuildSettingFacade                 settingsService)
        : base(logger)
    {
        _settingsService = settingsService;
    }

    [Command("set_timezone")]
    public async Task SetTimeZone(TimeZoneInfo tzInfo)
    {
        LogTrace($"Set TimeZone {tzInfo.DisplayName}");
        var guildId = Context.Guild.Id;

        await _settingsService.SetGuildTimezoneAsync(tzInfo.Id, guildId);

        await ReplyAsync($"Server timezone set to {tzInfo}");
    }

    [Command("get_timezone")]
    public async Task GetTimeZone()
    {
        LogTrace("Get TimeZone");
        var guildId = Context.Guild.Id;
        var tzInfo = await _settingsService.GetGuildTimezoneAsync(guildId);

        if (string.IsNullOrWhiteSpace(tzInfo)) return;

        await ReplyAsync($"Server timezone is {tzInfo}");
    }

    [Command("set_muterole")]
    public async Task SetMuteRole(SocketRole role)
    {
        LogTrace($"Set mute role to {role}");
        var guildId = Context.Guild.Id;

        await _settingsService.SetMuteRoleAsync(guildId, role.Id);

        await ReplyAsync($"Server mute role set to {role}");
    }

    [Command("get_muterole")]
    public async Task GetMuteRole()
    {
        LogTrace("Get guild mute role");
        var guild = Context.Guild;

        var settingsGuildMuteRole = await _settingsService.GetMuteRoleAsync(guild.Id);
        if (settingsGuildMuteRole is 0)
        {
            await ReplyAsync("There is no mute role set");
            return;
        }

        await ReplyAsync($"Server mute role is {settingsGuildMuteRole}");
    }
}