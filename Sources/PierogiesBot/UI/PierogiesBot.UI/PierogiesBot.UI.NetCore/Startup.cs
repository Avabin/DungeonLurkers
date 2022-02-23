using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.UI.Authentication;
using Shared.UI.IoC;
using Shared.UI.NetCore;

namespace PierogiesBot.UI.NetCore;

public class Startup
{
    public Startup(IConfiguration configuration, IHostEnvironment environment)
    {
        Configuration    = configuration;
        Environment = environment;
    }

    public IConfiguration   Configuration { get; }
    public IHostEnvironment Environment   { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddInfrastructure();

        services.AddSingleton<IAuthenticationStore, LiteDbAuthenticationStore>();
    }
}