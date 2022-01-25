using Microsoft.Extensions.Hosting;

namespace PierogiesBot.Discord.Infrastructure.Features.DiscordHost;

internal class DiscordHostedService : IHostedService
{
    private readonly IDiscordService _service;
    public DiscordHostedService(IDiscordService service) => _service = service;
    public async Task StartAsync(CancellationToken cancellationToken) => await _service.Start();
    public async Task StopAsync(CancellationToken cancellationToken) => await _service.Stop();
}