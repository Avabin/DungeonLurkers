using Discord;
using Discord.Interactions;

namespace PierogiesBot.Discord.Interactions.Features.BotCrontabRules;

public class EmojiAutocompleteHandler : AutocompleteHandler
{
    public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context,   IAutocompleteInteraction autocompleteInteraction,
                                                                        IParameterInfo      parameter, IServiceProvider         services)
    {
        var emotes = context.Guild.Emotes;
        
        
        var result = emotes.Take(25).Select(emoji => new AutocompleteResult($"{emoji}", emoji.Name));
            
        return Task.FromResult(AutocompletionResult.FromSuccess(result));
    }
}