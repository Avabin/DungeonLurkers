using System.Net;
using Discord.Interactions;
using Microsoft.Extensions.Logging;
using RestEase;

namespace PierogiesBot.Discord.Interactions.Features.Dices;

public class DicesInteractionModule : InteractionModuleBase
{
    private readonly IDicesApi                       _dicesApi;
    private readonly ILogger<DicesInteractionModule> _logger;

    public DicesInteractionModule(IDicesApi dicesApi, ILogger<DicesInteractionModule> logger)
    {
        _dicesApi    = dicesApi;
        _logger = logger;
    }

    [SlashCommand("roll", "Roll a dice ('xdy' where x is the number of dices and y is the number of faces)")]
    public async Task RollAsync(int diceCount, int sides)
    {
        try
        {
            var max = diceCount * sides;
        }
        catch (OverflowException)
        {
            await RespondAsync("That's too much. I'm not smart enough to compute that :(");
        }
        var expression = $"{diceCount}d{sides}";

        try
        {
            var result = await _dicesApi.RollAsync(expression);
            await RespondAsync(result.ToString());
        }
        catch (ApiException e) when (e.StatusCode is HttpStatusCode.BadRequest)
        {
            await RespondAsync("Invalid expression: " + e.Message, ephemeral: true);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while rolling dice");
            await RespondAsync("Something broke :(\r\nCheck the logs at https://seq.avabin.xyz/#/events?signal=signal-18,signal-33,signal-17&filter=Error%20while%20rolling%20dice");
        }
    }
}