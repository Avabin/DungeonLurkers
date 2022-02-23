using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace PierogiesBot.Discord.Interactions.Features.WhoIs;

public class WhoIsInteractionModule : InteractionModuleBase
{
    [SlashCommand("whois", "Check user full discord name")]
    public async Task WhoIsAsync(IMentionable? user)
    {
        await RespondAsync((user ?? Context.User).ToString());
    }
    
    [SlashCommand("ping", "Check bot latency")]
    public async Task PingAsync()
    {
        await RespondAsync("Pong!");
    }
}