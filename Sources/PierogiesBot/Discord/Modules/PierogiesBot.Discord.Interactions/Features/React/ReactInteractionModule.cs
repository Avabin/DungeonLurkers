using Discord;
using Discord.Interactions;
using PierogiesBot.Discord.Interactions.Features.BotResponseRules;

namespace PierogiesBot.Discord.Interactions.Features.React;

[RequireUserPermission(GuildPermission.AddReactions)]
public class ReactInteractionModule : InteractionModuleBase
{
    [SlashCommand("react", "React to last message")]
    public async Task React([Autocomplete(typeof(GuildEmojiAutocompleteHandler))] string reactionName)
    {
        var messagesBefore = await Context.Channel
                                          .GetMessagesAsync(Context.Interaction.Id, Direction.Before, 1)
                                          .FlattenAsync();

        var messageBefore = messagesBefore?.FirstOrDefault();

        if (messageBefore is null)
        {
            return;
        }

        var emote = Context
                   .Guild.Emotes
                   .FirstOrDefault(e => e.Name.Equals(reactionName, StringComparison.InvariantCultureIgnoreCase));

        if (emote is null)
        {
            return;
        }

        await messageBefore.AddReactionAsync(emote);

        await RespondAsync("Ok!", ephemeral: true);
    }
    
    [SlashCommand("react_to", "React to specified message")]
    public async Task React([Autocomplete(typeof(LastMessagesAutocompleteHandler))]    string  messageId,
                            [Autocomplete(typeof(GuildEmojiAutocompleteHandler))] string reactionName)
    {
        var messageSnowflakeId = ulong.Parse(messageId);
        var message = await Context.Channel.GetMessageAsync(messageSnowflakeId);

        var emote = Context
                   .Guild.Emotes
                   .FirstOrDefault(e => e.Name.Equals(reactionName, StringComparison.InvariantCultureIgnoreCase));

        if (emote is null)
        {
            return;
        }

        await message.AddReactionAsync(emote);
        
        await RespondAsync("Ok!", ephemeral: true);
    }
}

public class LastMessagesAutocompleteHandler : AutocompleteHandler
{
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context,   IAutocompleteInteraction autocompleteInteraction,
                                                                        IParameterInfo      parameter, IServiceProvider         services)
    {
        var messages = await context.Channel.GetMessagesAsync(context.Interaction.Id, Direction.Before, 25).FlattenAsync();

        var suggestions = messages.Select(m => new AutocompleteResult(m.Content, m.Id.ToString()));

        return AutocompletionResult.FromSuccess(suggestions);
    }
}