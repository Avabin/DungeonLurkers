using RestEase;

namespace PierogiesBot.Discord.Interactions.Features.Dices;

public interface IDicesApi
{
    [Get]
    Task<int> RollAsync([Query] string expression);
}