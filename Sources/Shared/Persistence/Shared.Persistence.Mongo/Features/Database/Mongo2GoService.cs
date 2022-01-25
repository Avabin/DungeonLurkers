using Microsoft.Extensions.Logging;
using Mongo2Go;

namespace Shared.Persistence.Mongo.Features.Database;

public class Mongo2GoService
{
    private readonly ILogger<Mongo2GoService> _logger;
    private          MongoDbRunner?           _instance;

    public Mongo2GoService(ILogger<Mongo2GoService> logger) => _logger = logger;

    public string StartMongo()
    {
        _logger.LogInformation("Starting Mongo2Go");
        _instance = MongoDbRunner.Start();

        return _instance.ConnectionString;
    }

    public void StopMongo()
    {
        _logger.LogInformation("Stopping Mongo2Go");
        _instance?.Dispose();
    }
}