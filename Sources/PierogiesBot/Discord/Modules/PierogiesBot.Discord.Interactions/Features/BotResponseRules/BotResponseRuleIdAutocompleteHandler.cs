using Discord;
using Discord.Interactions;
using PierogiesBot.Persistence.BotResponseRules.Features;

namespace PierogiesBot.Discord.Interactions.Features.BotResponseRules;

public class BotResponseRuleIdAutocompleteHandler : AutocompleteHandler
{
    private readonly IBotResponseRuleFacade _facade;

    public BotResponseRuleIdAutocompleteHandler(IBotResponseRuleFacade facade)
    {
        _facade = facade;
    }
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context,   IAutocompleteInteraction autocompleteInteraction,
                                                                              IParameterInfo      parameter, IServiceProvider         services)
    {
        var filter = autocompleteInteraction.Data.Current.Value as string ?? "{}";
        var documents =
            await _facade.GetAllByPredicateAsync(x => x.Responses.Any(x => x.StartsWith(filter)) || x.TriggerText.StartsWith(filter), limit: 25);
        
        var choices = documents.Select(x =>
        {
            var s = $"Trigger: \"{x.TriggerText}\", Responses = {{{string.Join(",", x.Responses)}}}";
            return s.Length > 99 ? new AutocompleteResult(s[..100], x.Id) : new AutocompleteResult(s, x.Id);
        });
        
        return AutocompletionResult.FromSuccess(choices);
    }
}