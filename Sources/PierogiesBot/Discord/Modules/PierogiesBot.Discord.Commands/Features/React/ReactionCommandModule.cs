using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using PierogiesBot.Discord.Commands.Features.Logging;

namespace PierogiesBot.Discord.Commands.Features.React;

[RequireUserPermission(GuildPermission.AddReactions)]
[Group("react")]
public class ReactionCommandModule : LoggingModuleBase<ICommandContext>
{
    public ReactionCommandModule(ILogger<ReactionCommandModule> logger) : base(logger)
    {
    }

    [Command]
    [Summary(                        "Reacts to last message before command with a specified reaction")]
    public async Task React([Summary("Reaction name")] string reactionName)
    {
        LogTrace($"React with {{{reactionName}}}");
        var messagesBefore = await Context.Channel
                                          .GetMessagesAsync(Context.Message, Direction.Before, 1)
                                          .FlattenAsync();

        var messageBefore = messagesBefore?.FirstOrDefault();

        if (messageBefore is null)
        {
            LogTrace($"Message before {Context.Message} not found");
            return;
        }

        var emote = Context
                   .Guild.Emotes
                   .FirstOrDefault(e => e.Name.Equals(reactionName, StringComparison.InvariantCultureIgnoreCase));

        if (emote is null)
        {
            LogTrace($"Emote {reactionName} not found");
            return;
        }

        await messageBefore.AddReactionAsync(emote);
    }

    [Command]
    [Summary(                        "Reacts to a given message with a specified reaction")]
    public async Task React([Summary("Message ID")]    ulong  messageSnowflakeId,
                            [Summary("Reaction name")] string reactionName)
    {
        LogTrace($"React to '{messageSnowflakeId}' with {reactionName}");
        var message = await Context.Channel.GetMessageAsync(messageSnowflakeId);

        var emote = Context
                   .Guild.Emotes
                   .FirstOrDefault(e => e.Name.Equals(reactionName, StringComparison.InvariantCultureIgnoreCase));

        if (emote is null)
        {
            LogTrace($"Emote {reactionName} not found");
            return;
        }

        await message.AddReactionAsync(emote);
    }
}