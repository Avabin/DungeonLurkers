using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using PierogiesBot.Discord.Commands.Features.Logging;
using PierogiesBot.Persistence.BotResponseRules.Features;
using PierogiesBot.Shared.Enums;
using PierogiesBot.Shared.Features.BotResponseRules;

namespace PierogiesBot.Discord.Commands.Features;

[RequireUserPermission(GuildPermission.Administrator)]
public class BotResponseRulesCommandModule : LoggingModuleBase<ICommandContext>
{
    private readonly IBotResponseRuleFacade _ruleFacade;

    public BotResponseRulesCommandModule(ILogger<BotResponseRulesCommandModule> logger, IBotResponseRuleFacade ruleFacade) : base(logger)
    {
        _ruleFacade = ruleFacade;
    }
    
    [Command("new_response")]
    [Summary("Adds a new response rule")]
    public async Task NewResponseRule(string trigger, string response, StringComparison comparison = StringComparison.InvariantCulture, bool isRegex = false, bool shouldTriggerOnContains = true, ResponseMode responseMode = ResponseMode.First)
    {
        var responseRule = new CreateBotResponseRuleDto
        {
            ShouldTriggerOnContains = shouldTriggerOnContains,
            IsTriggerTextRegex = isRegex,
            TriggerText = trigger,
            StringComparison = comparison,
            Responses = {response}
        };

        var result = await _ruleFacade.CreateAsync(responseRule);
        
        await Context.Channel.SendMessageAsync($"Added new response rule: {result.Id}");
    }
}