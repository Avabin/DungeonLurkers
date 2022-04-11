using Discord;
using Discord.Interactions;
using PierogiesBot.Persistence.BotResponseRules.Features;
using PierogiesBot.Shared.Enums;
using PierogiesBot.Shared.Features.BotResponseRules;
using Shared.Persistence.Core.Features.Exceptions;

namespace PierogiesBot.Discord.Interactions.Features.BotResponseRules;

[RequireUserPermission(GuildPermission.Administrator)]
public class BotResponseRuleInteractionModule : InteractionModuleBase
{
    private readonly IBotResponseRuleFacade _facade;

    public BotResponseRuleInteractionModule(IBotResponseRuleFacade facade)
    {
        _facade = facade;
    }

    [SlashCommand("new_response_rule", "Add new response rule to bot")]
    public async Task CreateNewResponseRuleAsync(string           trigger, string response,
                                                 StringComparison comparison = StringComparison.InvariantCulture,
                                                 bool             isRegex = false, bool shouldTriggerOnContains = true,
                                                 ResponseMode     responseMode = ResponseMode.First)
    {
        var responseRule = new CreateBotResponseRuleDto
        {
            ShouldTriggerOnContains = shouldTriggerOnContains,
            IsTriggerTextRegex      = isRegex,
            TriggerText             = trigger,
            StringComparison        = comparison,
            Responses               = { response },
            ResponseMode            = responseMode,
        };

        var result = await _facade.CreateAsync(responseRule);

        await RespondAsync($"Added new response rule: {result.Id}");
    }

    [SlashCommand("add_response", "Add new response to existing rule")]
    public async Task AddResponse([Autocomplete(typeof(BotResponseRuleIdAutocompleteHandler))] string ruleId,
                                  string                                                              response)
    {
        try
        {
            await _facade.AddResponseToRuleAsync(ruleId, response);

            await RespondAsync($"Added new response ({response}) to rule {ruleId}");
        }
        catch (DocumentNotFoundException)
        {
            await RespondAsync($"Rule with id {ruleId} not found");
        }
    }

    [SlashCommand("del_response", "Add new response rule to bot")]
    public async Task RemoveResponse([Autocomplete(typeof(BotResponseRuleIdAutocompleteHandler))] string ruleId,
                                     [Autocomplete(typeof(BotResponseRuleResponsesAutocompleteHandler))]
                                     string response)
    {
        try
        {
            await _facade.RemoveResponseFromRuleAsync(ruleId, response);

            await RespondAsync($"Added new response ({response}) to rule {ruleId}");
        }
        catch (DocumentNotFoundException)
        {
            await RespondAsync($"Rule {ruleId} not found");
        }
    }

    [SlashCommand("del_response_rule", "Delete bot response rule")]
    public async Task DeleteRule([Autocomplete(typeof(BotResponseRuleIdAutocompleteHandler))] string ruleId)
    {
        try
        {
            await _facade.DeleteAsync(ruleId);

            await RespondAsync($"Deleted rule {ruleId}");
        }
        catch (DocumentNotFoundException)
        {
            await RespondAsync($"Rule {ruleId} not found");
        }
    }
}