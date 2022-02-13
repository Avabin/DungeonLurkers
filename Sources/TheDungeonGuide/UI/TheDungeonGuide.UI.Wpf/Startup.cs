using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TheDungeonGuide.UI.Shared.Features.IoC;
using TheDungeonGuide.UI.Shared.Features.Navigation;

namespace TheDungeonGuide.UI.Wpf;

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
        services.AddInfrastructure(Environment, new Lazy<IServiceProvider>(() => App.Container));
        services.AddNavigation();
    }
}