using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using PierogiesBot.Discord.Commands.Features.Logging;

namespace PierogiesBot.Discord.Commands.Features.SendEmoji;

[RequireUserPermission(GuildPermission.SendMessages)]
[Group("emoji")]
public class EmojiCommandModule : LoggingModuleBase<ICommandContext>
{
    public EmojiCommandModule(ILogger<EmojiCommandModule> logger) : base(logger)
    {
    }

    [Command]
    [Summary(                        "Sends a single message containing specified emoji")]
    public async Task Emoji([Summary("Emoji")] string emoji)
    {
        LogTrace($"Emoji {emoji}");
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

    [Command]
    [Summary(                         "Sends a single message containing specified emojis")]
    public async Task Emojis([Summary("Emojis")] params string[] emojis)
    {
        LogTrace($"Emoji {{{string.Join(", ", emojis)}}}");
        var emotes = Context.Guild.Emotes;
        IEnumerable<GuildEmote?> msgEmotes = emojis.Select(s =>
                                                               emotes.FirstOrDefault(emote =>
                                                                   emote.Name
                                                                        .Equals(s,
                                                                                    StringComparison
                                                                                       .InvariantCultureIgnoreCase)));

        var guildEmotes    = msgEmotes.ToList();
        var notFoundEmotes = guildEmotes.Where(emote => emote is null).ToList();

        if (notFoundEmotes.Any())
        {
            await ReplyAsync($"Not found emotes: {string.Join(", ", notFoundEmotes)}");
            return;
        }

        var foundEmotes = guildEmotes.Where(x => x != null);

        await ReplyAsync(string.Join("", foundEmotes.Select(e => e!.ToString())));
    }
}