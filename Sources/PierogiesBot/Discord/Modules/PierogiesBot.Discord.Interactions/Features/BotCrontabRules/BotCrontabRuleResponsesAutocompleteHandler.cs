using Discord;
using Discord.Interactions;
using MongoDB.Bson;
using PierogiesBot.Persistence.BotCrontabRule.Features;
using PierogiesBot.Persistence.BotResponseRules.Features;

namespace PierogiesBot.Discord.Interactions.Features.BotCrontabRules;

public class BotCrontabRuleResponsesAutocompleteHandler : AutocompleteHandler
{
    private readonly IBotCrontabRuleFacade _facade;

    public BotCrontabRuleResponsesAutocompleteHandler(IBotCrontabRuleFacade facade)
    {
        _facade = facade;
    }

    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(
        IInteractionContext context,   IAutocompleteInteraction autocompleteInteraction,
        IParameterInfo      parameter, IServiceProvider         services)
    {
        var filter       = autocompleteInteraction.Data.Current.Value as string ?? "";
        var ruleIdOption = autocompleteInteraction.Data.Options.Single(x => x.Name == "rule-id");
        var ruleId       = ruleIdOption.Value as string;

        if (!ObjectId.TryParse(ruleId, out _))
            return AutocompletionResult.FromSuccess(Enumerable.Empty<AutocompleteResult>());


        var responses = await _facade.GetResponsesForRuleAsync(ruleId!);

        return AutocompletionResult.FromSuccess(responses.Where(x => x.StartsWith(filter, StringComparison.InvariantCultureIgnoreCase)).Select(x => x.Length > 99 ? new AutocompleteResult(x[..100], x) : new AutocompleteResult(x, x)));
    }
}