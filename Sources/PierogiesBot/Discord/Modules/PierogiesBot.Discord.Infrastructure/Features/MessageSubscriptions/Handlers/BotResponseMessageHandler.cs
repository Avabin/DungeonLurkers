using Discord.Commands;
using PierogiesBot.Discord.Core.Features.MessageSubscriptions.Handlers;
using PierogiesBot.Persistence.BotResponseRules.Features;
using PierogiesBot.Shared.Enums;
using PierogiesBot.Shared.Features.BotResponseRules;

namespace PierogiesBot.Discord.Infrastructure.Features.MessageSubscriptions.Handlers;

internal class BotResponseMessageHandler : IUserSocketMessageHandler
{
    private readonly IBotResponseRuleFacade _repository;
    private readonly Random _random;

    public BotResponseMessageHandler(IBotResponseRuleFacade repository)
    {
        _repository = repository;
        _random = new Random();
    }


    public async Task<IResult> HandleAsync(SocketCommandContext messageContext)
    {
        var rules = await _repository.GetAllAsync();
        var rule = rules.FirstOrDefault(r => r.CanExecuteRule(messageContext.Message.Content));
        if (rule is null)
            return ExecuteResult.FromError(CommandError.UnmetPrecondition, "No matching rule for given message");

        var ruleResponses = rule.Responses.ToList();
        switch (rule.ResponseMode)
        {
            case ResponseMode.Unknown:
                break;
            case ResponseMode.First:
                await messageContext.Channel.SendMessageAsync(ruleResponses.First());
                break;
            case ResponseMode.Random:
                await messageContext.Channel.SendMessageAsync(ruleResponses[_random.Next(ruleResponses.Count - 1)]);
                break;
            default:
                return ExecuteResult.FromError(new ArgumentOutOfRangeException(nameof(BotResponseRuleDto.ResponseMode),
                    "not known ResponseMode"));
        }

        return ExecuteResult.FromSuccess();
    }
}