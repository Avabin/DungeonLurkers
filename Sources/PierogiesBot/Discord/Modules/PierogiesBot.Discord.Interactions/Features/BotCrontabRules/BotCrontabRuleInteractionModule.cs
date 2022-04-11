using Discord;
using Discord.Interactions;
using MongoDB.Bson;
using PierogiesBot.Persistence.BotCrontabRule.Features;
using PierogiesBot.Shared.Enums;
using PierogiesBot.Shared.Features.BotCrontabRules;
using Shared.Persistence.Core.Features.Exceptions;

namespace PierogiesBot.Discord.Interactions.Features.BotCrontabRules;

[RequireUserPermission(GuildPermission.Administrator)]
public class BotCrontabRuleInteractionModule : InteractionModuleBase
{
    private readonly IBotCrontabRuleFacade _facade;

    public BotCrontabRuleInteractionModule(IBotCrontabRuleFacade facade)
    {
        _facade = facade;
    }

    [SlashCommand("new_crontab_rule", "Add new scheduled response rule to bot")]
    public async Task CreateNewResponseRuleAsync(string       crontab, string response,
                                                 bool         isEmoji      = false,
                                                 ResponseMode responseMode = ResponseMode.First)
    {
        var responseRule = new CreateBotCrontabRuleDto
        {
            Crontab       = crontab,
            IsEmoji       = isEmoji,
            ReplyMessages = { response },
            ResponseMode  = responseMode,
        };

        var result = await _facade.CreateAsync(responseRule);

        await RespondAsync($"Added new response rule: {result.Id}");
    }

    [SlashCommand("add_scheduled_response", "Add new response to existing rule")]
    public async Task AddResponse([Autocomplete(typeof(BotCrontabRuleIdAutocompleteHandler))] string ruleId,
                                  string                                                             response)
    {
        try
        {
            if (await ValidateId(ruleId)) return;
            await _facade.AddResponseToRuleAsync(ruleId, response);

            await RespondAsync($"Added new response ({response}) to rule {ruleId}");
        }
        catch (DocumentNotFoundException)
        {
            await RespondAsync($"Rule with id {ruleId} not found");
        }
    }

    [SlashCommand("add_scheduled_reaction", "Add new reaction to existing rule")]
    public async Task AddEmoji([Autocomplete(typeof(BotCrontabRuleIdAutocompleteHandler))] string ruleId,
                               [Autocomplete(typeof(EmojiAutocompleteHandler))]
                               string emoji)
    {
        try
        {
            if (await ValidateId(ruleId)) return;
            await _facade.AddEmojiToRuleAsync(ruleId, emoji);

            await RespondAsync($"Added new response ({emoji}) to rule {ruleId}");
        }
        catch (DocumentNotFoundException)
        {
            await RespondAsync($"Rule with id {ruleId} not found");
        }
    }

    private async Task<bool> ValidateId(string ruleId)
    {
        if (ObjectId.TryParse(ruleId, out _)) return false;
        await RespondAsync("Invalid rule id");
        return true;
    }

    [SlashCommand("del_scheduled_response", "Add new response rule to bot")]
    public async Task RemoveResponse([Autocomplete(typeof(BotCrontabRuleIdAutocompleteHandler))] string ruleId,
                                     [Autocomplete(typeof(BotCrontabRuleResponsesAutocompleteHandler))]
                                     string response)
    {
        try
        {
            if (await ValidateId(ruleId)) return;
            await _facade.RemoveResponseFromRuleAsync(ruleId, response);

            await RespondAsync($"Added new response ({response}) to rule {ruleId}");
        }
        catch (DocumentNotFoundException)
        {
            await RespondAsync($"Rule {ruleId} not found");
        }
    }

    [SlashCommand("del_scheduled_response_rule", "Delete bot response rule")]
    public async Task DeleteRule([Autocomplete(typeof(BotCrontabRuleIdAutocompleteHandler))] string ruleId)
    {
        try
        {
            if (await ValidateId(ruleId)) return;
            await _facade.DeleteAsync(ruleId);

            await RespondAsync($"Deleted rule {ruleId}");
        }
        catch (DocumentNotFoundException)
        {
            await RespondAsync($"Rule {ruleId} not found");
        }
    }
}