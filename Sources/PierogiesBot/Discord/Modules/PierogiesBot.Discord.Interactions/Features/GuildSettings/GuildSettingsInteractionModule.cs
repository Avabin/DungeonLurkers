using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Logging;
using PierogiesBot.Persistence.GuildSettings.Features;
using TimeZoneConverter;

namespace PierogiesBot.Discord.Interactions.Features.GuildSettings;

public class GuildSettingsInteractionModule : InteractionModuleBase
{
    private readonly ILogger<GuildSettingsInteractionModule> _logger;
    private readonly IGuildSettingFacade                     _settingsService;

    public GuildSettingsInteractionModule(ILogger<GuildSettingsInteractionModule> logger,
                                          IGuildSettingFacade                 settingsService)
    {
        _logger               = logger;
        _settingsService = settingsService;
    }
    
    [SlashCommand("set_timezone", "Set the guild time zone")]
    public async Task SetTimeZone([Autocomplete(typeof(TimeZoneAutocompleteHandler))] TimeZoneInfo tzInfo)
    {
        _logger.LogTrace($"Set TimeZone {tzInfo.DisplayName}");
        var guildId = Context.Guild.Id;

        await _settingsService.SetGuildTimezoneAsync(tzInfo.Id,guildId);

        await ReplyAsync($"Server timezone set to {tzInfo}");
    }

    [SlashCommand("get_timezone", "Get the guild time zone")]
    public async Task GetTimeZone()
    {
        _logger.LogTrace("Get TimeZone");
        var guildId = Context.Guild.Id;
        var tzInfo = await _settingsService.GetGuildTimezoneAsync(guildId);

        if (string.IsNullOrWhiteSpace(tzInfo))
        {
            await ReplyAsync("Nothing found");
            return;
        }

        await ReplyAsync($"Server timezone is {tzInfo}");
    }

    [SlashCommand("set_muterole", "Set the guild mute role")]
    public async Task SetMuteRole(IRole role)
    {
        _logger.LogTrace($"Set mute role to {role}");
        var guildId = Context.Guild.Id;

        await _settingsService.SetMuteRoleAsync(guildId,role.Id);

        await ReplyAsync($"Server mute role set to {role}");
    }

    [SlashCommand("get_muterole", "Get guild mute role")]
    public async Task GetMuteRole()
    {
        _logger.LogTrace("Get guild mute role");
        var guild = Context.Guild;
        var guildId = guild.Id;

        var settingsGuildMuteRole = await _settingsService.GetMuteRoleAsync(guildId);
        if (settingsGuildMuteRole is 0)
        {
            await ReplyAsync("There is no mute role set");
            return;
        }

        await ReplyAsync($"Server mute role is {settingsGuildMuteRole}");
    }
}

public class TimeZoneAutocompleteHandler : AutocompleteHandler
{
    public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context,   IAutocompleteInteraction autocompleteInteraction,
                                                                        IParameterInfo      parameter, IServiceProvider         services)
    {
        var filter = autocompleteInteraction.Data.Current.Value as string ?? "";
        var choices = TZConvert.KnownIanaTimeZoneNames.Where(x => x.StartsWith(filter)).Take(25).Select(x => new AutocompleteResult(x, x)).ToList();
        return Task.FromResult(AutocompletionResult.FromSuccess(choices));
    }
}