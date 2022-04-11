using Discord;
using Discord.Interactions;

namespace PierogiesBot.Discord.Interactions.Features.BotResponseRules;

public class GuildEmojiAutocompleteHandler : AutocompleteHandler
{
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(
        IInteractionContext context,   IAutocompleteInteraction autocompleteInteraction,
        IParameterInfo      parameter, IServiceProvider         services)
    {
        var emotes = await context.Guild.GetEmotesAsync();

        var autocompleteResults = emotes
                                 .OrderByDescending(x => x.CreatedAt)
                                 .Take(25)
                                 .Select(x => new AutocompleteResult(x.Name, x.Name));
        return emotes is null or {Count: 0}
                   ? AutocompletionResult.FromError(InteractionCommandError.Unsuccessful,
                                                    $"Unable to get emotes from guild {context.Guild}")
                   : AutocompletionResult.FromSuccess(autocompleteResults);
    }
}