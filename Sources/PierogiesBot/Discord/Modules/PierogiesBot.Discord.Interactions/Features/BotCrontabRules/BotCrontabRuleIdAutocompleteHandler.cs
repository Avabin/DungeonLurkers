using Discord;
using Discord.Interactions;
using PierogiesBot.Persistence.BotCrontabRule.Features;

namespace PierogiesBot.Discord.Interactions.Features.BotCrontabRules;

public class BotCrontabRuleIdAutocompleteHandler : AutocompleteHandler
{
    private readonly IBotCrontabRuleFacade _facade;

    public BotCrontabRuleIdAutocompleteHandler(IBotCrontabRuleFacade facade)
    {
        _facade = facade;
    }

    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(
        IInteractionContext context,   IAutocompleteInteraction autocompleteInteraction,
        IParameterInfo      parameter, IServiceProvider         services)
    {
        var filter = autocompleteInteraction.Data.Current.Value as string ?? "{}";
        var documents =
            await _facade
               .GetAllByPredicateAsync(x =>
                                           x.ReplyMessages.Any(x =>
                                                                   x.StartsWith(filter)) ||
                                           x.Crontab.StartsWith(filter)                  ||
                                           x.ReplyEmojis.Any(x => x.StartsWith(filter)), limit: 25);

        var choices = documents.Select(x =>
        {
            var s = $"Crontab:\"{x.Crontab}\",Responses{{{string.Join(",", x.ReplyMessages)},Emojis{{{string.Join(",", x.ReplyEmojis)}}}}}";
            return s.Length > 99 ? new AutocompleteResult(s[..100], x.Id) : new AutocompleteResult(s, x.Id);
        });

        return AutocompletionResult.FromSuccess(choices);
    }
}