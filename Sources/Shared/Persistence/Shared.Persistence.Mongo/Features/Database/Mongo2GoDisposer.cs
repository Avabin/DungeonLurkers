using Microsoft.Extensions.Hosting;

namespace Shared.Persistence.Mongo.Features.Database;

public class Mongo2GoDisposer : IHostedService
{
    private readonly Mongo2GoService _service;

    public Mongo2GoDisposer(Mongo2GoService service) => _service = service;

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _service.StopMongo();

        return Task.CompletedTask;
    }
}