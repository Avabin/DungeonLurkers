using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Logging;
using PierogiesBot.Discord.Interactions.Features.BotResponseRules;

namespace PierogiesBot.Discord.Interactions.Features.Emoji;

[RequireUserPermission(GuildPermission.SendMessages)]
public class EmojiInteractionModule : InteractionModuleBase
{
    private readonly ILogger<EmojiInteractionModule> _logger;

    public EmojiInteractionModule(ILogger<EmojiInteractionModule> logger)
    {
        _logger = logger;
    }

    
    [SlashCommand("emoji", "Tell bot to send emoji")]
    public async Task Emoji([Autocomplete(typeof(GuildEmojiAutocompleteHandler))] string emoji)
    {
        var emotes = Context.Guild.Emotes;
        var emote =
            emotes.FirstOrDefault(emote => emote.Name.Equals(emoji, StringComparison.InvariantCultureIgnoreCase));

        if (emote is null)
        {
            await ReplyAsync($"Not found emote: {emote}");
            return;
        }

        await ReplyAsync(emote.ToString());
    }
}