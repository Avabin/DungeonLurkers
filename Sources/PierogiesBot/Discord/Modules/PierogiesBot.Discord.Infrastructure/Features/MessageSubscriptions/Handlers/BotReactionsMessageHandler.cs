using Discord.Commands;
using PierogiesBot.Persistence.BotReactRules.Features;
using PierogiesBot.Shared.Features.BotResponseRules;

namespace PierogiesBot.Discord.Infrastructure.Features.MessageSubscriptions.Handlers;

internal class BotReactionsMessageHandler : IUserSocketMessageHandler
{
    private readonly IBotReactRuleFacade _service;

    public BotReactionsMessageHandler(IBotReactRuleFacade service)
    {
        _service = service;
    }

    public async Task<IResult> HandleAsync(CommandContext messageContext)
    {
        var rules = await _service.GetAllAsync();
        var rule = rules.FirstOrDefault(r => r.CanExecuteRule(messageContext.Message.Content));
        if (rule is null)
            return ExecuteResult.FromError(CommandError.UnmetPrecondition, "No matching rule for given message");

        var reactions = rule.Reactions.ToList();
        var reaction = reactions.First();
        var reactionEmote = messageContext.Guild.Emotes.FirstOrDefault(e => e.Name.Equals(reaction));

        if (reactionEmote is null)
            return ExecuteResult.FromError(CommandError.Unsuccessful, $"Emote {reaction} not found");

        await messageContext.Message.AddReactionAsync(reactionEmote);
        return ExecuteResult.FromSuccess();
    }
}